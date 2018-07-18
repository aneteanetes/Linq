using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Bars.Linq.Async
{
    public interface IAsyncEnumerable<T>
    {
        IAsyncEnumerator<T> GetAsyncEnumerator();

        Task ForEach(Action<T> result);

        Task<List<T>> ToList();
    }
}