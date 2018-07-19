namespace Bars.NuGet.Querying.Iterators
{
    using global::Bars.Linq.Async;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    public class AsyncEnumerator<T> : IAsyncEnumerator<T>
    {
        private IEnumerator<T> awaitedSourceEnumerator;
        private IEnumerator<Task<IEnumerable<T>>> sourceEnumerator;

        public AsyncEnumerator(IEnumerable<Task<IEnumerable<T>>> sources)
        {
            this.sourceEnumerator = sources
                .OrderByCompletion()
                .GetEnumerator();
        }

        public Task<T> CurrentAsync
        {
            get
            {
                if (awaitedSourceEnumerator == null)
                {
                    return Task.FromResult(default(T));
                }

                return Task.FromResult(awaitedSourceEnumerator.Current);
            }
        }

        /// <summary>
        /// рефактор потом, иначе пиздец
        /// </summary>
        /// <returns></returns>
        public async Task<bool> MoveNext()
        {
            if (awaitedSourceEnumerator != null)
            {
                var movedAwaitedSource = awaitedSourceEnumerator.MoveNext();
                if (!movedAwaitedSource)
                {
                    awaitedSourceEnumerator = null;
                    return await MoveNext();
                }
                return true;
            }

            var movedSources = sourceEnumerator.MoveNext();
            if (!movedSources)
            {
                return false;
            }

            awaitedSourceEnumerator = (await sourceEnumerator.Current)
                .GetEnumerator();

            return await MoveNext();

            //if (currentSourceEnumerator == null)
            //{
            //    var movedSources = sourceEnumerator.MoveNext();
            //    if (!movedSources)
            //    {
            //        awaitedSourceEnumerator = null;
            //        return false;
            //    }

            //    var z = await sourceEnumerator.Current;
            //}

            //awaitedSource = (await currentSource).ToList(); //IListEnumerator не финализирует, сучара

            //return await MoveNext();

            //if (TryMoveNextAwaitedSource(out var movedNew))
            //    return movedNew;

            //return false;
        }

        public void Dispose() { }
    }
}
