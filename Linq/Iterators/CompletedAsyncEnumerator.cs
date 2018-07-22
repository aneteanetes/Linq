namespace Bars.NuGet.Querying.Iterators
{
    using global::Bars.Linq.Async;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    /// <summary>
    /// async enumerator from completed source
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class CompletedAsyncEnumerator<T> : IAsyncEnumerator<T>
    {
        private IEnumerator<T> enumerator;

        public CompletedAsyncEnumerator(IEnumerable<T> sources)
        {
            this.enumerator = sources.GetEnumerator();
        }

        public Task<T> CurrentAsync
        {
            get
            {
                return Task.FromResult(enumerator.Current);
            }
        }
        
        public Task<bool> MoveNext()
        {
            return Task.FromResult(enumerator.MoveNext());
        }

        public void Dispose() { }
    }
}
