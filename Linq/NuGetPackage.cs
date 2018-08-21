namespace Bars.NuGet.Querying
{
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using global::Bars.NuGet.Querying.Types;

    /// <summary>
    /// NuGet package with file paths
    /// </summary>
    [DebuggerDisplay("id: {Id} | version: {Version.ToString()} | Unpacked: {Unpacked}")]
    public sealed class NuGetPackage : NuGetPackageInfo
    {
        public IEnumerable<string> Items
            => Dll
            .Concat(Pdb)
            .Concat(Xml)
            .Concat(Content);

        public IEnumerable<string> Pdb { get; internal set; } = new List<string>();

        public IEnumerable<string> Dll { get; internal set; } = new List<string>();

        public IEnumerable<string> Xml { get; internal set; } = new List<string>();

        public IEnumerable<string> Content { get; internal set; } = new List<string>();

        public bool Unpacked { get; internal set; }

        internal readonly NuGetQueryFilter Filter = new NuGetQueryFilter();
    }
}