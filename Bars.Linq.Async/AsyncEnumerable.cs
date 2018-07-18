using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Bars.Linq.Async
{
    public class AsyncEnumerable<T> : IAsyncEnumerable<T>
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

        /// <summary>
        /// это вот пока что
        /// </summary>
        /// <param name="result"></param>
        public async Task ForEach(Action<T> result)
        {
            var enumerator = this.GetAsyncEnumerator();
            while (await enumerator.MoveNext())
            {
                var current = await enumerator.CurrentAsync;
                result(current);
            }
        }
    }
}