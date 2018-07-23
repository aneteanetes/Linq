namespace Bars.NuGet.Querying.Types
{
    using System;
    using System.Collections.Generic;

    public class NuGetPackageInfo
    {
        public string Id { get; set; }

        internal string PackageId { get; set; }

        public Version Version { get; set; }

        public string Title { get; set; }

        public ISet<string> Tags { get; set; }

        public string Author { get; set; }

        public string Description { get; set; }

        public string Summary { get; set; }

        public string Owner { get; set; }
    }
}