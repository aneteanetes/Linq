namespace Bars.NuGet.Querying.Types
{
    using System.Collections.Generic;
    using System.Runtime.Versioning;

    /// <summary>
    /// Search filter for nuget repository
    /// </summary>
    public sealed class NuGetQueryFilter
    {
        public Dictionary<string, string> Filter { get; set; } = new Dictionary<string, string>();

        public int Skip { get; set; }

        public int Take { get; set; }
        
        public bool IncludePrerelease { get; set; }
        
        public bool IncludeDelisted { get; set; }
                
        public bool Latest { get; set; }

        public IdOrderRule OrderById { get; set; } = IdOrderRule.None;

        public ICollection<FrameworkName> SupportedFrameworks { get; set; } = new List<FrameworkName>();

        public bool SyncIncompatibility { get; set; }
    }
}
