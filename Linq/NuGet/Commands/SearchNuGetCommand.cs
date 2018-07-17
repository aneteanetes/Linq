namespace Bars.NuGet.Querying.NuGet.Commands
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.Versioning;
    using System.Threading.Tasks;
    using global::Bars.NuGet.Querying.Commands;
    using global::Bars.NuGet.Querying.Types;
    using global::NuGet.Protocol.Core.Types;

    internal class SearchNuGetCommand : BaseNuGetCommand<PackageSearchResource>
    {
        public Task<IEnumerable<IPackageSearchMetadata>> SearchMeta(NuGetQueryFilter queryFilter)
        {
            return this.Exec().Call(async resource =>
            {
                var filter = this.ConvertFilter(queryFilter);
                filter.SupportedFrameworks = this.ConvertFrameworkNames(queryFilter.SupportedFrameworks);
                return await resource.SearchAsync(queryFilter.Filter, filter, queryFilter.Skip, queryFilter.Take, this.nugetLogger, this.cancellationToken);
            });
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

        private IEnumerable<string> ConvertFrameworkNames(IEnumerable<FrameworkName> frameworkNames)
        {
            return frameworkNames.Select(frmNme =>
            {
                var takeVersionNums = frmNme.Version.Build == 0 ? 2 : 3;
                return frmNme.Identifier
                    .ToLowerInvariant()
                    .Replace(" ", "") 
                + frmNme.Version.ToString(takeVersionNums);
            });
        }
    }
}