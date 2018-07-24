namespace Bars.NuGet.Querying.Client
{
    using global::Bars.NuGet.Querying.Types;
    using global::NuGet.Protocol.Core.Types;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    public partial class NuGetRepository : IDisposable
    {
        public IEnumerable<Task<IEnumerable<bool>>> Remove(NuGetPackage nuGetQueryFilter)
        {
            return this.Exec<bool>(async (accessor) =>
            {
                await accessor.GetResource<PackageUpdateResource>().Delete(null, null, null, null, false, this.loggerAdapter);

                return Enumerable.Empty<bool>();
            });
        }
    }
}