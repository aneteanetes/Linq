//namespace NuGet.Querying.NuGet
//{
//    using global::NuGet.Configuration;
//    using global::NuGet.Frameworks;
//    using global::NuGet.Packaging;
//    using global::NuGet.Packaging.Core;
//    using global::NuGet.Protocol.Core.Types;
//    using global::NuGet.Versioning;
//    using MoreLinq;
//    using System;
//    using System.Collections.Generic;
//    using System.IO;
//    using System.Linq;
//    using System.Net;
//    using System.Threading;

//    public partial class NuGetRepository
//    {
//        /// <summary>
//        /// Поиск информации в репозитории
//        /// </summary>
//        /// <param name="search">Строка поиска, по умолчанию: <see cref="String.Empty"/></param>
//        /// <param name="filterType">Тип фильтра, подробнее: <see cref="SearchFilterType"/></param>
//        /// <param name="includePrerelease">Включать пред-релизные пакеты</param>
//        /// <param name="supportedFrameworks">Поддерживаемые версии фреймворков для каждого пакета</param>
//        /// <param name="packageTypes">Типы пакетов</param>
//        /// <param name="skip">Пропустить N (paging)</param>
//        /// <param name="take">Взять N (paging)</param>
//        /// <returns></returns>
//        public IEnumerable<IPackageSearchMetadata> SearchMeta(string search = "", SearchFilterType filterType = SearchFilterType.IsLatestVersion, bool includePrerelease = false, IEnumerable<string> supportedFrameworks = null, IEnumerable<string> packageTypes = null, int skip = 0, int take = 0)
//        {
//            IEnumerable<IPackageSearchMetadata> Search(PackageSearchResource resource)
//            {
//                var filter = new SearchFilter(includePrerelease, filterType);

//                if (supportedFrameworks == null || !supportedFrameworks.Any())
//                {
//                    if (Frameworks != null)
//                        supportedFrameworks = Frameworks.Select(f => f.ToString());
//                    else
//                        supportedFrameworks = new string[] { "netstandard2.1" };
//                }
//                filter.SupportedFrameworks = supportedFrameworks;

//                if (packageTypes != null)
//                {
//                    filter.PackageTypes = packageTypes;
//                }

//                if (take == 0)
//                    take = int.MaxValue;

//                return Sync(resource.SearchAsync(search, filter, skip, take, this.logger, CancellationToken.None));
//            }

//            return ExecSplit<PackageSearchResource, IEnumerable<IPackageSearchMetadata>>(Search, Collection);
//        }

//        /// <summary>
//        /// Скачать nupkg файл
//        /// </summary>
//        /// <param name="identity">Идентификационная информация пакета</param>
//        /// <param name="destinationPath">Путь куда сохранить пакет на жестком диске</param>
//        /// <returns></returns>
//        public bool DownloadNupkg(PackageIdentity identity, string destinationPath)
//        {
//            bool Download(FindPackageByIdResource resource)
//            {
//                var downloadCtx = new PackageDownloadContext(this.sourceCacheContext);
//                var directory = Path.GetDirectoryName(destinationPath);

//                var result = false;

//                using (var newFile = File.Create(destinationPath))
//                {
//                    result = Sync(resource.CopyNupkgToStreamAsync(identity.Id, identity.Version, newFile, this.sourceCacheContext, this.logger, CancellationToken.None));
//                }

//                return result;
//            }

//            return ExecSplit<FindPackageByIdResource, bool>(Download, OneIsSuccess);
//        }

//        /// <summary>
//        /// Прочитать информацию о содержимом пакета из репозиториев, возможно это будет происходить удалённо
//        /// </summary>
//        /// <param name="identity">Идентификационная информация пакета</param>
//        /// <returns>Асинхронный ридер информации о содержимом</returns>
//        public IAsyncPackageContentReader ReadPackage(PackageIdentity identity)
//        {
//            IAsyncPackageContentReader Read(FindPackageByIdResource resource)
//            {
//                return Sync(resource.GetPackageDownloaderAsync(identity, this.sourceCacheContext, this.logger, CancellationToken.None))
//                    .ContentReader;
//            }

//            return ExecSplit<FindPackageByIdResource, IAsyncPackageContentReader>(Read, First);
//        }

//        /// <summary>
//        /// Прочитать пакет из файла
//        /// </summary>
//        /// <param name="identity">Идентификационная информация пакета</param>так пу
//        /// <param name="filePath"></param>
//        /// <returns>Асинхронный ридер информации о содержимом</returns>
//        public IAsyncPackageContentReader ReadPackage(PackageIdentity identity, string filePath)
//        {
//            var downloader = new LocalPackageArchiveDownloader(Path.GetDirectoryName(filePath), filePath, identity, this.logger);
//            return downloader.ContentReader;
//        }

//        /// <summary>
//        /// Выгружает все файлы из пакета в директорию     
//        /// Возвращает пути c директорией.
//        /// Пример для пакета NuGet.Protocol.Core.Types и директории C:\
//        /// C:\NuGet\Protocol\Core\Types\Type.dll,
//        /// C:\NuGet\Protocol\Core\Types\Type.xml   
//        /// </summary>
//        /// <param name="directory">Директория куда</param>
//        /// <param name="id">Идентификатор пакета</param>
//        /// <param name="version">Версия пакета</param>
//        /// <param name="frameworks">Поддерживаемые версии фреймворков для каждого пакета</param>
//        /// <returns></returns>
//        public IEnumerable<string> ExtractNupkg(string directory, string id, NuGetVersion version, IEnumerable<NuGetFramework> frameworks = null)
//            => ExtractNupkg(directory, new PackageIdentity(id, version), frameworks);

//        /// <summary>
//        /// Выгружает все файлы из пакета в директорию     
//        /// Возвращает пути c директорией.
//        /// Пример для пакета NuGet.Protocol.Core.Types и директории C:\
//        /// C:\NuGet\Protocol\Core\Types\Type.dll,
//        /// C:\NuGet\Protocol\Core\Types\Type.xml   
//        /// </summary>
//        /// <param name="directory">Директория куда</param>
//        /// <param name="identity">Идентификационная информация пакета</param>так пу
//        /// <param name="frameworks">Поддерживаемые версии фреймворков для каждого пакета</param>
//        /// <returns></returns>
//        public IEnumerable<string> ExtractNupkg(string directory, PackageIdentity identity, IEnumerable<NuGetFramework> frameworks = null)
//        {
//            IEnumerable<string> Unpack(FindPackageByIdResource resource, PackageSource packageSource)
//            {
//                string InternalUnpack(string sourcePath, string targetPath, Stream sourceStream)
//                {
//                    var dir = Path.GetDirectoryName(targetPath);
//                    if (!Directory.Exists(dir))
//                    {
//                        Directory.CreateDirectory(dir);
//                    }

//                    using (var targetStream = File.OpenWrite(targetPath))
//                    {
//                        sourceStream.CopyTo(targetStream);
//                    }

//                    return targetPath;
//                }

//                try
//                {
//                    if (frameworks == null)
//                        frameworks = this.Frameworks;

//                    var loader = Sync(resource.GetPackageDownloaderAsync(identity, this.sourceCacheContext, this.logger, CancellationToken.None));

//                    var infoReader = loader.ContentReader;
//                    var nupkgReader = loader.CoreReader;
                    
//                    var packageItems = Sync(infoReader.GetLibItemsAsync(CancellationToken.None))
//                        .Concat(Sync(infoReader.GetContentItemsAsync(CancellationToken.None)))
//                        .ToArray();

//                    var frameworkItems = packageItems
//                        .Where(libInfo => frameworks.Contains(libInfo.TargetFramework));


//                    var mapFolderPaths = frameworkItems
//                        .Select(libInfo => new
//                        {
//                            physicalPath = Path.Combine(
//                                    directory,
//                                    string.Join(Path.DirectorySeparatorChar.ToString(), identity.Id.Split(".")),
//                                    identity.Version.ToString()),
//                            relativePath = libInfo.Items
//                        });

//                    var libsGroupedByDirectory = mapFolderPaths.GroupBy(x => x.physicalPath);


//                    List<string> newPaths = new List<string>();
//                    foreach (var libDir in libsGroupedByDirectory)
//                    {
//                        var newPath = Sync(nupkgReader.CopyFilesAsync(libDir.Key, libDir.SelectMany(x => x.relativePath), InternalUnpack, this.logger, CancellationToken.None));
//                        newPaths.AddRange(newPath);
//                    }

//                    return newPaths;
//                }
//                catch(IOException dirNotFound)
//                {
//                    throw new IOException("Невозможно сохранить скачанный пакет", dirNotFound);
//                }
//                catch (Exception ex)
//                {
//                    throw new InvalidOperationException("Ошибка при распаковке сборок чаще всего связанна с тем что в текущих репозиториях нет такой сборки", ex);
//                }
//            }

//            return ExecSplit<FindPackageByIdResource, IEnumerable<string>>(Unpack, Collection);
//        }
//    }
//}