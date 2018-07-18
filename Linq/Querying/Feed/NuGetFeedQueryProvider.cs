namespace Bars.NuGet.Querying.Feed
{
    using global::Bars.Linq.Async;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;

    internal class NuGetFeedQueryProvider : IAsyncQueryProvider<NuGetPackage>
    {
        private readonly IEnumerable<string> feeds;

        public NuGetFeedQueryProvider(string[] feeds)
        {
            this.feeds = feeds;
        }

        public IAsyncQueryable<NuGetPackage> AsyncExecute(Expression expression)
        {
            return NuGetFeedQueryMaterializer.Execute<NuGetPackage>(expression, true, feeds);
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