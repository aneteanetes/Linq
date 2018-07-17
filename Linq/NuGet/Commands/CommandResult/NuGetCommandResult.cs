namespace Bars.NuGet.Querying.Commands.CommandResult
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

        public Task<IEnumerable<T>> Call<T>(Func<TResource, Task<IEnumerable<T>>> func)
        {
            var bind = Binder.Bind(this.resources(), func);
            var combined = Binder.TaskCombine(bind);
            var merged = Binder.TaskMerge(combined);

            return merged;
        }
    }
}