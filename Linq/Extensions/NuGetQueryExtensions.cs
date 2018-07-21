namespace Bars.NuGet.Querying
{
    using Bars.NuGet.Querying.Types;
    using global::Bars.Linq.Async;
    using global::Bars.NuGet.Querying.Iterators;
    using global::Bars.NuGet.Querying.Patches;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.Versioning;
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
        /// Packages with target framework
        /// </summary>
        /// <param name="feedQuery"></param>
        /// <param name="netFramework"></param>
        /// <param name="version"></param>
        /// <returns></returns>
        public static IQueryable<NuGetPackage> ForFramework(this IQueryable<NuGetPackage> feedQuery, NetFramework netFramework, string version)
            => ForFramework(feedQuery, netFramework, new Version(version));

        /// <summary>
        /// Packages with target framework
        /// </summary>
        /// <param name="feedQuery"></param>
        /// <param name="netFramework"></param>
        /// <param name="majorVersion"></param>
        /// <returns></returns>
        public static IQueryable<NuGetPackage> ForFramework(this IQueryable<NuGetPackage> feedQuery, NetFramework netFramework, int majorVersion)
            => ForFramework(feedQuery, netFramework, new Version(majorVersion, 0));

        /// <summary>
        /// Packages with target framework
        /// </summary>
        /// <param name="feedQuery"></param>
        /// <param name="netFramework"></param>
        /// <param name="majorVersion"></param>
        /// <param name="minorVersion"></param>
        /// <returns></returns>
        public static IQueryable<NuGetPackage> ForFramework(this IQueryable<NuGetPackage> feedQuery, NetFramework netFramework, int majorVersion, int minorVersion)
            => ForFramework(feedQuery, netFramework, new Version(majorVersion, minorVersion));

        /// <summary>
        /// Packages with target framework
        /// </summary>
        /// <param name="feedQuery"></param>
        /// <param name="netFramework"></param>
        /// <param name="majorVersion"></param>
        /// <param name="minorVersion"></param>
        /// <param name="buildVersion"></param>
        /// <returns></returns>
        public static IQueryable<NuGetPackage> ForFramework(this IQueryable<NuGetPackage> feedQuery, NetFramework netFramework, int majorVersion, int minorVersion, int buildVersion)
            => ForFramework(feedQuery, netFramework, new Version(majorVersion, minorVersion, buildVersion));

        /// <summary>
        /// Packages with target framework
        /// </summary>
        /// <param name="feedQuery"></param>
        /// <param name="netFramework"></param>
        /// <param name="version"></param>
        /// <returns></returns>
        public static IQueryable<NuGetPackage> ForFramework(this IQueryable<NuGetPackage> feedQuery, NetFramework netFramework, Version version)
        {
            var framework = new FrameworkName($".{netFramework.ToString().ToLowerInvariant()}", version);
            return feedQuery.Where(x => x.Filter.SupportedFrameworks.Contains(framework));
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