namespace Bars.Linq.Async
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
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

        public static IAsyncQueryable<T> AsAsyncQueryable<T>(this IQueryable<T> queryable)
        {
            if (typeof(IAsyncQueryable<>).IsAssignableFrom(queryable.GetType()))
            {
                return queryable as IAsyncQueryable<T>;
            }

            throw new InvalidCastException($"{queryable.GetType()} is not instance of {typeof(IAsyncQueryable<>).FullName} and can not be casted to it!");
        }
    }
}
