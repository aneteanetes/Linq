namespace Bars.Linq.Async
{
    using System;
    using System.Threading.Tasks;

    public interface IAsyncEnumerator<T> : IDisposable
    {
        Task<T> CurrentAsync { get; }

        Task<bool> MoveNext();
    }
}