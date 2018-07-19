namespace Bars.NuGet.Querying.Feed
{
    using global::Bars.Linq.Async;
    using global::Bars.NuGet.Querying.Client;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;

    internal class NuGetFeedQueryProvider : IAsyncQueryProvider<NuGetPackage>
    {
        private readonly NuGetRepository NuGetRepository;

        public NuGetFeedQueryProvider(string[] feeds)
        {
            this.NuGetRepository = new NuGetRepository(feeds);
        }

        public IAsyncQueryable<NuGetPackage> AsyncExecute(Expression expression)
        {
            return NuGetFeedQueryMaterializer.Execute(expression, true, NuGetRepository);
        }

        public IAsyncQueryable<NuGetPackage> CreateAsyncQuery(Expression expression)
        {
            return new NuGetFeed(this, expression);
        }

        //public IAsyncEnumerable<NuGetPackage> AsyncExecute(Expression expression)
        //{
        //    var isEnumerable = (typeof(TResult).Name == "IEnumerable`1");
        //    return NuGetFeedQueryMaterializer.Execute(expression, isEnumerable, feeds);
        //}

        public IQueryable CreateQuery(Expression expression)
        {
            return new NuGetFeed(this, expression);
        }

        public IQueryable<TElement> CreateQuery<TElement>(Expression expression)
        {
            return (IQueryable<TElement>)new NuGetFeed(this, expression);
        }

        public object Execute(Expression expression)
        {
            return Execute<NuGetPackage>(expression);
        }

        public TResult Execute<TResult>(Expression expression)
        {
            return default;
            //var isEnumerable = (typeof(TResult).Name == "IEnumerable`1");
            //return (TResult)NuGetFeedQueryMaterializer.Execute(expression, isEnumerable, feeds);
        }
    }
}