namespace Bars.Linq.Async
{
    using System.Collections;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    //public class AsyncEnumerator
    //{
    //    public static AsyncEnumerator<T> FromResult<T>(IEnumerable<Task<IEnumerable<T>>> data) where T : IEnumerable
    //    {
    //        return new AsyncEnumerator<T>
    //        {
    //            enumerable = data
    //        };
    //    }
    //}

    public class AsyncEnumerator<T> : IAsyncEnumerator<T>
    {
        private IEnumerable<T> awaitedSource;
        private Task<IEnumerable<T>> currentSource;
        private IEnumerable<Task<IEnumerable<T>>> sources;

        public AsyncEnumerator(IEnumerable<Task<IEnumerable<T>>> sources)
        {
            this.sources = sources;
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
            if (awaitedSource != null)
            {
                var movedAwaitedSource = awaitedSource.GetEnumerator().MoveNext();
                if (!movedAwaitedSource)
                {
                    awaitedSource = null;
                    currentSource = null;
                    return false;
                }
            }

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

            var movedAwaitedSource2 = awaitedSource.GetEnumerator().MoveNext();
            if (!movedAwaitedSource2)
            {
                awaitedSource = null;
                currentSource = null;
                return false;
            }

            return true;
        }

        public void Dispose() { }
    }


    //public class AsyncEnumerator<T> : AsyncEnumerator, IAsyncEnumerator<T> where T: IEnumerable
    //{
    //    internal T enumerable;

    //    public Task<T> CurrentAsync
    //    {
    //        get
    //        {
    //            var current = this.enumerable.GetEnumerator().Current;
    //            return (Task<T>)current;
    //        }
    //    }

    //    public Task<bool> MoveNext()
    //    {
    //        return Task.FromResult(this.enumerable.GetEnumerator().MoveNext());
    //    }

    //    public void Dispose() { }
    //}
}