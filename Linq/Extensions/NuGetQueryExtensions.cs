namespace Bars.NuGet.Querying
{
    using System.Linq;

    public static class NuGetQueryExtensions
    {
        public static IQueryable<NuGetPackage> IncludePrerelease(this NuGetFeed feed)
        {
            feed.CurrentFilter.IncludePrerelease = true;
            return feed;
        }

        public static IQueryable<NuGetPackage> IncludePrerelease(this IQueryable<NuGetPackage> feedQuery)
        {
            if (feedQuery is NuGetFeed nugetFeed)
            {
                return IncludePrerelease(nugetFeed);
            }

            return feedQuery;
        }

        public static IQueryable<NuGetPackage> IncludeUnlisted(this NuGetFeed feed)
        {
            feed.CurrentFilter.IncludeDelisted = true;
            return feed;
        }

        public static IQueryable<NuGetPackage> IncludeUnlisted(this IQueryable<NuGetPackage> feedQuery)
        {
            if (feedQuery is NuGetFeed nugetFeed)
            {
                return IncludeUnlisted(nugetFeed);
            }

            return feedQuery;
        }

        public static IQueryable<NuGetPackage> Latest(this NuGetFeed feed)
        {
            feed.CurrentFilter.Latest = true;
            return feed;
        }

        public static IQueryable<NuGetPackage> Latest(this IQueryable<NuGetPackage> feedQuery)
        {
            if (feedQuery is NuGetFeed nugetFeed)
            {
                return Latest(nugetFeed);
            }

            return feedQuery;
        }
    }
}