namespace Bars.NuGet.Querying.Iterators
{
    using global::Bars.Linq.Async;

    public class AsyncEnumerable
    {
        public static AsyncEnumerable<T> FromResult<T>(IAsyncEnumerator<T> data)
        {
            return new AsyncEnumerable<T>(data);
        }
    }

    public class AsyncEnumerable<T> : AsyncEnumerable, IAsyncEnumerable<T>
    {
        private readonly IAsyncEnumerator<T> asyncEnumerator;

        public AsyncEnumerable(IAsyncEnumerator<T> asyncEnumerator)
        {
            this.asyncEnumerator = asyncEnumerator;
        }

        internal AsyncEnumerable()
        {
        }

        public IAsyncEnumerator<T> GetAsyncEnumerator() => this.asyncEnumerator;
    }
}
