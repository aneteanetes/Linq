namespace Bars.NuGet.Querying
{
    using global::Bars.NuGet.Querying.Patches;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
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

        public static Task<List<NuGetPackage>> ToListAsync(this IQueryable<NuGetPackage> feedQuery)
        {
            return Task.FromResult(feedQuery.ToList());
        }

        //public static AsyncEnumerable<IQueryable<NuGetPackage>> Async(this IQueryable<NuGetPackage> feedQuery)
        //{
        //    return new AsyncEnumerable<IQueryable<NuGetPackage>>(feedQuery);
        //}
    }
}