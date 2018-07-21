namespace Bars.Linq.Async
{
    using System.Linq;
    using System.Linq.Expressions;

    public interface IAsyncQueryProvider<T> : IQueryProvider
    {
        IAsyncQueryable<T> AsyncExecute(Expression expression);

        IAsyncQueryable<T> CreateAsyncQuery(Expression expression);
    }
}