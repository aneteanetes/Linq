namespace Bars.NuGet.Querying.Feed
{
    using global::Bars.Linq.Async;
    using global::Bars.NuGet.Querying.Client;
    using global::Bars.NuGet.Querying.Iterators;
    using global::Bars.NuGet.Querying.Types;
    using global::NuGet.Protocol.Core.Types;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Threading.Tasks;

    internal class NuGetFeedQueryMaterializer
    {
        internal static IAsyncQueryable<NuGetPackage> Execute(Expression expression, bool isEnumerable, NuGetRepository nuGetRepository)
        {
            var queryableElements = Root(nuGetRepository);

            // Copy the expression tree that was passed in, changing only the first
            // argument of the innermost MethodCallExpression.
            var treeCopier = new NuGetFeedQueryVisitor(queryableElements);
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

        private static IAsyncQueryable<NuGetPackage> Root(NuGetRepository nuGetRepository)
        {
            var filter = new NuGetQueryFilter
            {
                Filter = "bars"
            };

            var metaRequests = nuGetRepository.Search.Exec(filter);

            var convertedRequests = ConvertRequest(metaRequests);

            var enumer = new AsyncEnumerator<NuGetPackage>(convertedRequests);
            
            var enumerable = AsyncEnumerable.FromResult(enumer);

            return default;
        }

        private static IEnumerable<Task<IEnumerable<NuGetPackage>>> ConvertRequest(IEnumerable<Task<IEnumerable<IPackageSearchMetadata>>> metaRequests)
        {
            return metaRequests.Select(metaRequest => metaRequest.ContinueWith(request => FromTaskResult(request.Result)));
        }

        private static IEnumerable<NuGetPackage> FromTaskResult(IEnumerable<IPackageSearchMetadata> meta)
        {
            return meta.Select(FromMeta);
        }

        private static NuGetPackage FromMeta(IPackageSearchMetadata meta)
        {
            return new NuGetPackage
            {
                Id = meta.Identity.Id
            };
        }
    }
}
