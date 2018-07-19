namespace Bars.NuGet.Querying.Iterators
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Runtime.CompilerServices;
    using System.Threading.Tasks;
    using global::Bars.Linq.Async;

    public class AsyncEnumerable
    {
        public static AsyncEnumerable<T> FromResult<T>(IAsyncEnumerator<T> data)
        {
            return new AsyncEnumerable<T>(data);
        }
    }

    public class AsyncEnumerable<T> : AsyncEnumerable, IAsyncQueryable<T>
    {
        private readonly IAsyncEnumerator<T> asyncEnumerator;

        public AsyncEnumerable(IAsyncEnumerator<T> asyncEnumerator)
        {
            this.asyncEnumerator = asyncEnumerator;
        }

        public IAsyncQueryProvider<T> AsyncProvider { get; private set; }

        public Type ElementType => GetType();

        public Expression Expression => Expression.Constant(this);

        public IQueryProvider Provider { get; }

        public IAsyncEnumerator<T> GetAsyncEnumerator() => this.asyncEnumerator;

        public IEnumerator<T> GetEnumerator() => Enumerable.Empty<T>().GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();
    }
}