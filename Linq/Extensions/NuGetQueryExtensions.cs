namespace Bars.NuGet.Querying
{
    using global::Bars.Linq.Async;
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

        public static IAsyncEnumerable<NuGetPackage> ToAsync(this IQueryable<NuGetPackage> feedQuery)
        {
            if (feedQuery is NuGetFeed nugetFeed)
            {
                return new AsyncEnumerable<NuGetPackage>(nugetFeed.GetAsyncEnumerator());
            }

            throw new System.Exception("not async");
        }

        //public static AsyncEnumerable<IQueryable<NuGetPackage>> Async(this IQueryable<NuGetPackage> feedQuery)
        //{
        //    return new AsyncEnumerable<IQueryable<NuGetPackage>>(feedQuery);
        //}
    }
}