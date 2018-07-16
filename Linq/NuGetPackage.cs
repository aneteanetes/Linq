namespace Linq
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Text;

    public abstract class NuGetPackage
    {
        public string Id { get; set; }

        public IEnumerable<Stream> Items { get; set; }
    }
}