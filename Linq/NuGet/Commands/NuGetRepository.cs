namespace Bars.NuGet.Querying.Client
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using global::NuGet.Configuration;
    using global::NuGet.Frameworks;
    using global::NuGet.Protocol;
    using global::NuGet.Protocol.Core.Types;
    using global::Bars.NuGet.Querying.Logging;
    using global::Bars.NuGet.Querying.NuGet.Commands;

    public partial class NuGetRepository : IDisposable
    {
        public NuGetRepository()
        {
            this.Search = new SearchNuGetCommand(this.Repositories, this.loggerAdapter);
        }

        internal readonly SearchNuGetCommand Search;
    }
}