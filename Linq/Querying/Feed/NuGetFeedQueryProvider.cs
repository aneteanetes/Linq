namespace Bars.NuGet.Querying.Feed
{
    using global::Bars.Linq.Async;
    using global::Bars.NuGet.Querying.Client;
    using Microsoft.Extensions.Logging;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;

    internal class NuGetFeedQueryProvider : IAsyncQueryProvider<NuGetPackage>
    {
        private readonly NuGetRepository NuGetRepository;
        private readonly Expression root;

        public NuGetFeedQueryProvider(NuGetRepository nuGetRepository, Expression root)
        {
            this.NuGetRepository = nuGetRepository;
            this.root = root;
        }

        public IAsyncQueryable<NuGetPackage> AsyncExecute(Expression expression)
        {
            return NuGetFeedQueryMaterializer.Execute(expression, NuGetRepository, root);
        }

        public IAsyncQueryable<NuGetPackage> CreateAsyncQuery(Expression expression)
        {
            return new NuGetFeed(this, expression);
        }

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
            return Execute<IEnumerable<NuGetPackage>>(expression);
        }

        public TResult Execute<TResult>(Expression expression)
        {
            var result = NuGetFeedQueryMaterializer.Execute(expression, NuGetRepository, root) as IAsyncEnumerable<NuGetPackage>;
            var task = result.ToList();
            task.Wait();

            return (TResult)(task.Result as object);
        }
    }
}