namespace Bars.NuGet.Querying.Commands
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using global::Bars.NuGet.Querying.Functionality;
    using global::NuGet.Protocol.Core.Types;

    public class NuGetCommandResult<TResource> where TResource : class, INuGetResource
    {
        protected readonly Func<IEnumerable<TResource>> resources;

        public NuGetCommandResult(Func<IEnumerable<TResource>> resources)
        {
            this.resources = resources;
        }

        public IEnumerable<Task<IEnumerable<T>>> Call<T>(Func<TResource, Task<IEnumerable<T>>> func) => Binder.Bind(this.resources(), func);
    }
}