namespace Bars.NuGet.Querying.Iterators
{
    using global::Bars.Linq.Async;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public class AsyncEnumerator<T> : IAsyncEnumerator<T>
    {
        private IEnumerable<T> awaitedSource;
        private Task<IEnumerable<T>> currentSource;
        private IEnumerable<Task<IEnumerable<T>>> sources;

        public AsyncEnumerator(IEnumerable<Task<IEnumerable<T>>> sources)
        {
            this.sources = sources.OrderByCompletion();
        }

        public Task<T> CurrentAsync
        {
            get
            {
                if (awaitedSource == null)
                {
                    return Task.FromResult(default(T));
                }

                return Task.FromResult(awaitedSource.GetEnumerator().Current);
            }
        }

        /// <summary>
        /// рефактор потом, иначе пиздец
        /// </summary>
        /// <returns></returns>
        public async Task<bool> MoveNext()
        {
            if (TryMoveNextAwaitedSource(out var moved))
                return moved;

            if (currentSource == null)
            {
                var sourcesEnumerator = this.sources.GetEnumerator();
                var movedSources = sourcesEnumerator.MoveNext();
                if (!movedSources)
                {
                    currentSource = null;
                    return false;
                }

                currentSource = sourcesEnumerator.Current;
            }

            awaitedSource = await currentSource;

            if (TryMoveNextAwaitedSource(out var movedNew))
                return movedNew;

            return false;
        }

        private bool TryMoveNextAwaitedSource(out bool moveResult)
        {
            moveResult = false;

            if (awaitedSource == null)
                return false;

            var movedAwaitedSource = awaitedSource.GetEnumerator().MoveNext();
            if (!movedAwaitedSource)
            {
                awaitedSource = null;
                currentSource = null;
                return false;
            }
            moveResult = true;

            return true;
        }

        public void Dispose() { }
    }
}
