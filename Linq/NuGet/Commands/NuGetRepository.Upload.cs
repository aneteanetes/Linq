namespace Bars.NuGet.Querying.Client
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using global::NuGet.Protocol.Core.Types;

    public partial class NuGetRepository : IDisposable
    {
        /// <summary>
        /// feature
        /// </summary>
        /// <param name="nuGetQueryFilter"></param>
        /// <returns></returns>
        internal IEnumerable<Task<IEnumerable<bool>>> Upload(NuGetPackage nuGetQueryFilter)
        {
            return this.Exec<bool>(async (accessor) =>
            {
                await accessor.GetResource<PackageUpdateResource>().Push(null, null, 0, false, null, null, false, this.loggerAdapter);

                return Enumerable.Empty<bool>();
            });
        }
    }
}