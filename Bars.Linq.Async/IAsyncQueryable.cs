namespace Bars.Linq.Async
{
    using System.Linq;

    public interface IAsyncQueryable<T> : IOrderedQueryable<T>
    {
        IAsyncQueryProvider<T> AsyncProvider { get; }

        IAsyncEnumerator<T> GetAsyncEnumerator();
    }
}
