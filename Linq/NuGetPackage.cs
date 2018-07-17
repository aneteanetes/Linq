namespace Bars.NuGet.Querying
{
    using global::Bars.NuGet.Querying.Types;
    using System.Collections.Generic;
    using System.IO;

    public class NuGetPackage : NuGetPackageInfo
    {        
        public IEnumerable<Stream> Items { get; set; }
    }
}