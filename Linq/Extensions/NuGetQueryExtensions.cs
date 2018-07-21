namespace Bars.NuGet.Querying
{
    using global::Bars.Linq.Async;
    using global::Bars.NuGet.Querying.Iterators;
    using global::Bars.NuGet.Querying.Patches;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    public static class NuGetQueryExtensions
    {
        /// <summary>
        /// Includes not released versions of packages
        /// </summary>
        /// <param name="feedQuery"></param>
        /// <returns></returns>
        public static IQueryable<NuGetPackage> IncludePrerelease(this IQueryable<NuGetPackage> feedQuery)
        {
            return feedQuery.Where(jops => jops.Filter.IncludePrerelease == true);
        }

        /// <summary>
        /// Includes packages wich does not showing in public search
        /// </summary>
        /// <param name="feedQuery"></param>
        /// <returns></returns>
        public static IQueryable<NuGetPackage> IncludeUnlisted(this IQueryable<NuGetPackage> feedQuery)
        {
            return feedQuery.Where(x => x.Filter.IncludeDelisted == true);
        }

        /// <summary>
        /// Get latest versions of packages
        /// </summary>
        /// <param name="feedQuery"></param>
        /// <returns></returns>
        public static IQueryable<NuGetPackage> Latest(this IQueryable<NuGetPackage> feedQuery)
        {
            return feedQuery.Where(x => x.Filter.Latest == true);
        }

        /// <summary>
        /// Get async enumerable
        /// </summary>
        /// <param name="feedQuery"></param>
        /// <returns></returns>
        public static IAsyncEnumerable<NuGetPackage> ToAsync(this IQueryable<NuGetPackage> feedQuery)
        {
            if (feedQuery is NuGetFeed nugetFeed)
            {
                return new AsyncEnumerable<NuGetPackage>(nugetFeed.GetAsyncEnumerator());
            }

            throw new System.Exception("not async");
        }
    }
}