namespace Bars.NuGet.Querying.Feed
{
    using global::Bars.Linq.Async;
    using global::Bars.NuGet.Querying.Client;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;

    internal class NuGetFeedQueryMaterializer
    {
        internal static IAsyncQueryable<T> Execute<T>(Expression expression, bool isEnumerable, IEnumerable<string> feeds)
        {
            var queryableElements = Root<T>(feeds);

            // Copy the expression tree that was passed in, changing only the first
            // argument of the innermost MethodCallExpression.
            var treeCopier = new NuGetFeedQueryVisitor<T>(queryableElements);
            Expression newExpressionTree = treeCopier.Visit(expression);

            // This step creates an IQueryable that executes by replacing Queryable methods with Enumerable methods.
            if (isEnumerable)
            {
                return queryableElements.AsyncProvider.CreateAsyncQuery(newExpressionTree);
            }
            else
            {
                return queryableElements.AsyncProvider.AsyncExecute(newExpressionTree);
            }
        }

        private static IAsyncQueryable<T> Root<T>(IEnumerable<string> feeds)
        {
            return null;

            //var repo = new NuGetRepository(feeds);

            //var list = new List<NuGetPackage>();

            //return Enumerable.Empty<NuGetPackage>().AsQueryable();

            //return repo.SearchMeta().Select(x => new NuGetPackage
            //{
            //    Id = x.Identity.Id
            //}).AsQueryable();            
        }
    }
}
