namespace Bars.Linq.Async
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public static  class AsyncEnumerableExtensions
    {
        /// <summary>
        /// это вот пока что
        /// </summary>
        /// <param name="result"></param>
        public static async Task ForEach<T>(this IAsyncEnumerable<T> asyncEnumerable, Action<T> result)
        {
            var enumerator = asyncEnumerable.GetAsyncEnumerator();
            while (await enumerator.MoveNext())
            {
                var current = await enumerator.CurrentAsync;
                result(current);
            }
        }

        public static async Task<List<T>> ToList<T>(this IAsyncEnumerable<T> asyncEnumerable)
        {
            List<T> data = new List<T>();

            var enumerator = asyncEnumerable.GetAsyncEnumerator();
            while (await enumerator.MoveNext())
            {
                var current = await enumerator.CurrentAsync;
                data.Add(current);
            }

            return data;
        }

        public static IAsyncQueryable<T> AsQueryable<T>(this IAsyncEnumerable<T> asyncEnumerable)
        {
            return default;
        }
    }
}
