namespace Bars.NuGet.Querying.Iterators
{
    using System.Collections;
    using System.Collections.Generic;

    /// <summary>
    /// internal
    /// </summary>
    /// <typeparam name="T"></typeparam>
    internal class AsyncEnumerable<T> : IEnumerable<T>
    {
        private readonly AsyncEnumerator<T> enumerator;

        public AsyncEnumerable(AsyncEnumerator<T> enumerator)
        {
            this.enumerator = enumerator;
        }

        public IEnumerator<T> GetEnumerator() => this.enumerator;

        IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();
    }
}