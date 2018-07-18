namespace Bars.Linq.Async
{
    using System.Linq;
    using System.Linq.Expressions;

    public interface IAsyncQueryProvider<T> : IQueryProvider
    {
        /// <summary>
        /// object?
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        IAsyncQueryable<T> AsyncExecute(Expression expression);

        IAsyncQueryable<T> CreateAsyncQuery(Expression expression);

        //IAsyncQueryable<T> CreateAsyncQuery(Expression expression);

        //public IQueryable CreateQuery(Expression expression)
        //{
        //    throw new NotImplementedException();
        //}

        //public IQueryable<TElement> CreateQuery<TElement>(Expression expression)
        //{
        //    throw new NotImplementedException();
        //}

        //public object Execute(Expression expression)
        //{
        //    throw new NotImplementedException();
        //}

        //public TResult Execute<TResult>(Expression expression)
        //{
        //    throw new NotImplementedException();
        //}
    }
}