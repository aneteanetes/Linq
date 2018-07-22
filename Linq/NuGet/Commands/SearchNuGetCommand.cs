namespace Bars.NuGet.Querying.NuGet.Commands
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.Versioning;
    using System.Threading;
    using System.Threading.Tasks;
    using global::Bars.NuGet.Querying.Commands;
    using global::Bars.NuGet.Querying.Types;
    using global::NuGet.Protocol.Core.Types;

    internal class SearchNuGetCommand : BaseNuGetCommand<IPackageSearchMetadata, NuGetQueryFilter, PackageSearchResource>
    {
        public SearchNuGetCommand(IEnumerable<SourceRepository> sourceRepositories, global::NuGet.Common.ILogger nugetLogger) : base(sourceRepositories, nugetLogger)
        {
        }

        protected override async Task<IEnumerable<IPackageSearchMetadata>> Command(NuGetQueryFilter param, PackageSearchResource command)
        {
            var filter = this.ConvertFilter(param);
            var query = this.GetQuery(param.Filter);
            filter.SupportedFrameworks = this.ConvertFrameworkNames(param.SupportedFrameworks);
            return await command.SearchAsync(query, filter, param.Skip, param.Take, this.nugetLogger, this.cancellationToken);
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
            searchFilter.PackageTypes = queryFilter.PackageTypes;


            return searchFilter;
        }

        private string GetQuery(Dictionary<string,string> segments)
        {
            return string.Join(' ', segments.Select(x => $"{x.Key}:{x.Value}"));
        }

        private IEnumerable<string> ConvertFrameworkNames(IEnumerable<FrameworkName> frameworkNames)
        {
            return frameworkNames.Select(frmNme =>
            {
                var takeVersionNums = frmNme.Version.Build == -1 ? 2 : 3;
                return frmNme.Identifier
                    .ToLowerInvariant()
                    .Replace(" ", "")
                + frmNme.Version.ToString(takeVersionNums);
            });
        }
    }
}