using Bars.NuGet.Querying.Functionality;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Bars.NuGet.Querying.Patches
{
    public interface IAsyncEnumerable<T>
    {
        /// <summary>
        /// Gets an asynchronous enumerator over the sequence.
        /// </summary>
        /// <returns>Enumerator for asynchronous enumeration over the sequence.</returns>
        IAsyncEnumerator<T> GetEnumerator();
    }

    public interface IAsyncEnumerator<T>
    {
        T Current { get; }

        Task<bool> MoveNext();
    }

    //public interface IAsyncEnumerableAccessor<out T>
    //{
    //    IAsyncEnumerable<T> AsyncEnumerable { get; }
    //}

    //public interface IAsyncQueryProvider : IQueryProvider
    //{
    //    IAsyncEnumerable<TResult> ExecuteAsync<TResult>(Expression expression);

    //    Task<TResult> ExecuteAsync<TResult>(Expression expression, CancellationToken cancellationToken);
    //}

    public class AsyncEnumerable<T>
    {
        private readonly IQueryable<Task<T>> collection;

        public AsyncEnumerable(IQueryable<Task<T>> sources)
        {
            collection = sources;
        }

        public async void ForEach(Action<T> action)
        {
            foreach (var task in collection)
            {
                var value = await task;

                action(value);
            }
        }

        public Task<IEnumerable<T>> ToListAsync()
        {
            return Binder.TaskCombine(collection);
        }
    }
}