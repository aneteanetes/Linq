namespace Bars.NuGet.Querying.Client
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Bars.NuGet.Querying.NuGet.Commands;

    public partial class NuGetRepository : IDisposable
    {
        /// <summary>
        /// Executes func for every repository
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="func"></param>
        /// <returns></returns>
        protected IEnumerable<Task<IEnumerable<TResult>>> Exec<TResult>(Func<NuGetResourceAccessor, Task<IEnumerable<TResult>>> func)
        {
            List<Task<IEnumerable<TResult>>> execResults = new List<Task<IEnumerable<TResult>>>();

            foreach (var repository in this.Repositories)
            {
                var task = func(new NuGetResourceAccessor(repository));
                execResults.Add(task);
            }

            return execResults;
        }
    }
}