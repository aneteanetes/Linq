using NuGet.Protocol.Core.Types;
using System;
using System.Collections.Generic;
using System.Text;

namespace Bars.NuGet.Querying.Types
{
    public class NuGetQueryFilter
    {
        public string Filter { get; set; }



        public NuGetQueryFilter()
        {
            SearchFilter
            //string searchTerm, SearchFilter filters, int skip, int take, ILogger log, CancellationToken cancellationToken
        }
    }
}
