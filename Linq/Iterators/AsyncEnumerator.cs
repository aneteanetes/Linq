namespace Bars.NuGet.Querying.Iterators
{
    using System.Collections;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    /// <summary>
    /// internal
    /// </summary>
    /// <typeparam name="T"></typeparam>
    internal class AsyncEnumerator<T> : IEnumerator<T>
    {
        private IEnumerator<T> defferedEnumerator;
        private IEnumerator<Task<IEnumerable<T>>> sourceEnumerator;

        public AsyncEnumerator(IEnumerable<Task<IEnumerable<T>>> sources)
        {
            this.sourceEnumerator = sources
                .OrderByCompletion()
                .GetEnumerator();
        }
        
        public T Current
        {
            get
            {
                if (defferedEnumerator == null)
                {
                    return default;
                }

                return defferedEnumerator.Current;
            }
        }

        object IEnumerator.Current => this.Current;

        public bool MoveNext()
        {
            if (defferedEnumerator != null)
            {
                var movedAwaitedSource = defferedEnumerator.MoveNext();
                if (!movedAwaitedSource)
                {
                    defferedEnumerator.Dispose();
                    defferedEnumerator = null;
                    return MoveNext();
                }
                return true;
            }

            var movedSources = sourceEnumerator.MoveNext();
            if (!movedSources)
            {
                return false;
            }

            var current = sourceEnumerator.Current;
            current.Wait();

            defferedEnumerator = current.Result.GetEnumerator();

            return MoveNext();
        }

        public void Reset() => this.defferedEnumerator.Reset();

        public void Dispose()
        {
            if (this.defferedEnumerator != null)
                this.defferedEnumerator.Dispose();
        }
    }
}
