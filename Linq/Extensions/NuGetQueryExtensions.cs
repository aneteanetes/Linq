namespace Bars.NuGet.Querying
{
    using System;
    using System.Linq;
    using System.Runtime.Versioning;

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
        /// Searchs package with target tag
        /// </summary>
        /// <param name="feedQuery"></param>
        /// <param name="tag"></param>
        /// <returns></returns>
        public static IQueryable<NuGetPackage> WithTag(this IQueryable<NuGetPackage> feedQuery, string tag)
        {
            return feedQuery.Where(x => x.Tags.Contains(tag));
        }

        /// <summary>
        /// searches packages with target tags
        /// </summary>
        /// <param name="feedQuery"></param>
        /// <param name="tags"></param>
        /// <returns></returns>
        public static IQueryable<NuGetPackage> WithTags(this IQueryable<NuGetPackage> feedQuery, params string[] tags)
        {
            foreach (var tag in tags)
            {
                feedQuery = feedQuery.Where(x => x.Tags.Contains(tag));
            }

            return feedQuery;
        }
        

        /// <summary>
        /// Indicates that not applicable IQueryable methods will be executed after all async calls combined to one synchronous collection
        /// without this all not applicable IQueryable methods will be ignored
        /// </summary>
        /// <param name="feedQuery"></param>
        /// <returns></returns>
        public static IQueryable<NuGetPackage> SyncIncompatibility(this IQueryable<NuGetPackage> feedQuery)
        {
            return feedQuery.Where(x => x.Filter.SyncIncompatibility == true);
        }
    }
}