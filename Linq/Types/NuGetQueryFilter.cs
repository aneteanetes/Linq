namespace Bars.NuGet.Querying.Types
{
    using System.Collections.Generic;
    using System.Runtime.Versioning;

    internal class NuGetQueryFilter
    {
        public string Filter { get; set; }

        public int Skip { get; set; }

        public int Take { get; set; }
        
        public bool IncludePrerelease { get; set; }
        
        public bool IncludeDelisted { get; set; }
        
        public IEnumerable<string> PackageTypes { get; set; }
        
        public bool Latest { get; set; }

        public IdOrderRule OrderById { get; set; } = IdOrderRule.None;

        public IEnumerable<FrameworkName> SupportedFrameworks { get; set; }
    }
}
