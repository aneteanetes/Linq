namespace Bars.NuGet.Querying
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    public static class NuGetQueryExtensions
    {
        public static IQueryable<NuGetPackage> IncludePrerelease(this IQueryable<NuGetPackage> feedQuery)
        {
            return feedQuery.Where(x => x.Filter.IncludePrerelease == true);
        }

        public static IQueryable<NuGetPackage> IncludeUnlisted(this IQueryable<NuGetPackage> feedQuery)
        {
            return feedQuery.Where(x => x.Filter.IncludeDelisted == true);
        }

        public static IQueryable<NuGetPackage> Latest(this IQueryable<NuGetPackage> feedQuery)
        {
            return feedQuery.Where(x => x.Filter.Latest == true);
        }

        public static Task<IQueryable<NuGetPackage>> Async(this IQueryable<NuGetPackage> feedQuery)
        {
            return Task.FromResult<IQueryable<NuGetPackage>>(feedQuery);
        }

        public static Task<T> Async<T>(this T feedQuery) where T : IEnumerable<NuGetPackage>
        {
            return Task.FromResult<T>(feedQuery);
        }
    }
}