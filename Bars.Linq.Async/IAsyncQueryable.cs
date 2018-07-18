namespace Bars.Linq.Async
{
    using System.Linq;

    public interface IAsyncQueryable<T> : IAsyncEnumerable<T>, IOrderedQueryable<T>
    {
        IAsyncQueryProvider<T> AsyncProvider { get; }
    }
}
