namespace NuGet.Querying
{
    using System.Collections.Generic;
    using System.IO;

    public class NuGetPackage
    {
        public string Id { get; set; }

        public IEnumerable<Stream> Items { get; set; }
    }
}