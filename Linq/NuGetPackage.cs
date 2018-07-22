namespace Bars.NuGet.Querying
{
    using global::Bars.NuGet.Querying.Types;
    using System.Collections.Generic;
    using System.IO;

    public sealed class NuGetPackage : NuGetPackageInfo
    {        
        public IEnumerable<Stream> Items { get; set; }
        
        internal readonly NuGetQueryFilter Filter = new NuGetQueryFilter();
    }
}