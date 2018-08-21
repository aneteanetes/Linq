namespace Bars.NuGet.Querying.Client
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Runtime.Versioning;
    using System.Threading;
    using System.Threading.Tasks;
    using global::Bars.NuGet.Querying.Types;
    using global::NuGet.Frameworks;
    using global::NuGet.Packaging.Core;
    using global::NuGet.Protocol;
    using global::NuGet.Protocol.Core.Types;
    using global::NuGet.Versioning;

    public partial class NuGetRepository : IDisposable
    {
        public IEnumerable<Task<IEnumerable<NuGetPackage>>> Search(NuGetQueryFilter nuGetQueryFilter)
        {
            return this.Exec<NuGetPackage>(async (accessor) =>
            {                
                var search = await this.InternalSearch(nuGetQueryFilter, accessor.GetResource<PackageSearchResource>());

                var packages = search.Select(FromMetadata);

                List<NuGetPackage> downloadedPackages = new List<NuGetPackage>();
                foreach (var package in packages)
                {
                    downloadedPackages.Add(await DownloadFiles(package, nuGetQueryFilter.SupportedFrameworks, accessor.GetResource<FindPackageByIdResource>()));
                }
                return downloadedPackages;
            });
        }

        #region convert

        private NuGetPackage FromMetadata(IPackageSearchMetadata metadata)
        {
            return new NuGetPackage
            {
                Id = metadata.Identity.Id,
                Author = metadata.Authors,
                Description = metadata.Description,
                Owner = metadata.Owners,
                Summary = metadata.Summary,
                Tags = new HashSet<string>(metadata.Tags.Split(", ", StringSplitOptions.RemoveEmptyEntries)),
                Title = metadata.Title,
                Version = metadata.Identity.Version.Version
            };
        }

        #endregion

        #region search metadata

        private Task<IEnumerable<IPackageSearchMetadata>> InternalSearch(NuGetQueryFilter param, PackageSearchResource command)
        {
            var filter = this.ConvertFilter(param);
            var query = this.GetQuery(param.Filter);
            filter.SupportedFrameworks = this.ConvertFrameworkNames(param.SupportedFrameworks);
            return command.SearchAsync(query, filter, param.Skip, param.Take, this.loggerAdapter, CancellationToken.None);
        }

        private SearchFilter ConvertFilter(NuGetQueryFilter queryFilter)
        {
            SearchFilter searchFilter = null;

            if (queryFilter.Latest)
            {
                searchFilter = new SearchFilter(queryFilter.IncludePrerelease, queryFilter.IncludePrerelease
                    ? SearchFilterType.IsAbsoluteLatestVersion
                    : SearchFilterType.IsLatestVersion);
            }
            else
            {
                searchFilter = new SearchFilter(queryFilter.IncludePrerelease);
            }

            if (queryFilter.OrderById == IdOrderRule.Asc)
            {
                searchFilter.OrderBy = SearchOrderBy.Id;
            }

            searchFilter.IncludeDelisted = queryFilter.IncludeDelisted;

            return searchFilter;
        }

        private string GetQuery(Dictionary<string, string> segments)
        {
            return string.Join(' ', segments.Select(x => $"{x.Key}:{x.Value /*"\"" + x.Value + "\""*/}"));
        }
        
        private IEnumerable<string> ConvertFrameworkNames(IEnumerable<FrameworkName> frameworkNames)
        {
            return frameworkNames.Select(frameworkName =>
            {
                var takeVersionNums = frameworkName.Version.Build == -1 ? 2 : 3;
                var version = frameworkName.Version.ToString(takeVersionNums);

                var name = frameworkName.Identifier
                    .ToLowerInvariant()
                    .Replace(" ", "");

                return name + version;
            });
        }

        #endregion

        #region download files

        private async Task<NuGetPackage> DownloadFiles(NuGetPackage nuGetPackage, IEnumerable<FrameworkName> frameworks, FindPackageByIdResource resource)
        {
            var nuGetFrameworks = frameworks.Select(f => new NuGetFramework(f.Identifier.ToLowerInvariant().Replace(" ", ""), f.Version));

            var identity = new PackageIdentity(nuGetPackage.Id, new NuGetVersion(nuGetPackage.Version));
            var loader = await resource.GetPackageDownloaderAsync(identity, sourceCacheContext, loggerAdapter, CancellationToken.None);

            var resolvedFilePath = this.pathResolver.ResolveFilePath(nuGetPackage);
            var resolvedPath = Path.GetDirectoryName(resolvedFilePath);

            if (!Directory.Exists(resolvedPath))
            {
                Directory.CreateDirectory(resolvedPath);
            }            

            if (typeof(RemotePackageArchiveDownloader).IsAssignableFrom(loader.GetType()))
            {
                await loader.CopyNupkgFileToAsync(resolvedFilePath, CancellationToken.None);
            }

            var infoReader = loader.ContentReader;
            var nupkgReader = loader.CoreReader;

            var refs = await infoReader.GetReferenceItemsAsync(CancellationToken.None);
            var libs = await infoReader.GetLibItemsAsync(CancellationToken.None);
            var build = await infoReader.GetBuildItemsAsync(CancellationToken.None);
            var tools = await infoReader.GetToolItemsAsync(CancellationToken.None);
            var legacyContent = await infoReader.GetContentItemsAsync(CancellationToken.None);

            var allGropus = refs.ToArray()
                .Concat(libs.ToArray())
                .Concat(build.ToArray())
                .Concat(tools.ToArray())
                .Concat(legacyContent.ToArray())
                .Distinct()
                .ToArray();

            var items = allGropus.Where(dataGroup => nuGetFrameworks.Contains(dataGroup.TargetFramework))
                .SelectMany(group => group.Items)
                .ToArray();

            if (items.Length == 0)
            {
                return nuGetPackage;
            }

            Dictionary<string, List<string>> FileMap = new Dictionary<string, List<string>>()
            {
                {".pdb", new List<string>() },
                {".xml", new List<string>() },
                {".dll", new List<string>() },
                {".content", new List<string>() }
            };

            string InternalUnpack(string sourcePath, string targetPath, Stream sourceStream)
            {
                var targetDir = Path.GetDirectoryName(targetPath);
                if (!Directory.Exists(targetDir))
                {
                    Directory.CreateDirectory(targetDir);
                }

                using (var targetStream = File.OpenWrite(targetPath))
                {
                    sourceStream.CopyTo(targetStream);
                }

                var ext = Path.GetExtension(targetPath).ToLowerInvariant();
                if (FileMap.TryGetValue(ext, out var list))
                {
                    list.Add(targetPath);
                }
                else
                {
                    FileMap[".content"].Add(targetPath);
                }

                return targetPath;
            }

            await nupkgReader.CopyFilesAsync(resolvedPath, items, InternalUnpack, loggerAdapter, CancellationToken.None);

            nuGetPackage.Pdb = FileMap[".pdb"];
            nuGetPackage.Xml = FileMap[".xml"];
            nuGetPackage.Dll = FileMap[".dll"];
            nuGetPackage.Content = FileMap[".content"];
            nuGetPackage.Unpacked = true;

            return nuGetPackage;
        }

        #endregion
    }
}