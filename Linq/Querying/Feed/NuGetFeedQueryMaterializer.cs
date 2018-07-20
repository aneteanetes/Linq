namespace Bars.NuGet.Querying.Feed
{
    using Bars.NuGet.Querying.Visitors;
    using global::Bars.Linq.Async;
    using global::Bars.NuGet.Querying.Client;
    using global::Bars.NuGet.Querying.Iterators;
    using global::Bars.NuGet.Querying.Types;
    using global::NuGet.Protocol.Core.Types;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Runtime.Versioning;
    using System.Threading.Tasks;

    internal class NuGetFeedQueryMaterializer
    {
        internal static IAsyncQueryable<NuGetPackage> Execute(Expression expression, bool isEnumerable, NuGetRepository nuGetRepository)
        {
            var expr = new NuGetFeedQueryVisitor().CopyAndModify(expression);

            var filter = MaterializeFilter(expr);
            //тут мы подключаем всяких визиторов, которые будут формировать нормальный NuGetQueryFilter, передаём их в Root метод, который возвращает нам
            // трушную хуйню

            var queryableElements = Root(nuGetRepository, filter);

            //между этими двумя шагами мы подключаем всяких визиторов, которые будут формировать нормальный NuGetQueryFilter

            return queryableElements;

            // Copy the expression tree that was passed in, changing only the first
            // argument of the innermost MethodCallExpression.
            //var treeCopier = new NuGetFeedQueryVisitor(queryableElements);
            //Expression newExpressionTree = treeCopier.Visit(expression);

            //// This step creates an IQueryable that executes by replacing Queryable methods with Enumerable methods.
            //if (isEnumerable)
            //{
            //    return queryableElements.AsyncProvider.CreateAsyncQuery(newExpressionTree);
            //}
            //else
            //{
            //    return queryableElements.AsyncProvider.AsyncExecute(newExpressionTree);
            //}
        }

        private static NuGetQueryFilter MaterializeFilter(Expression expression)
        {
            return (expression as LambdaExpression).Compile().DynamicInvoke() as NuGetQueryFilter;
        }

        private static IAsyncQueryable<NuGetPackage> Root(NuGetRepository nuGetRepository, NuGetQueryFilter filter)
        {
            filter = filter ?? new NuGetQueryFilter
            {
                Filter = "bars",
                SupportedFrameworks = new FrameworkName[] { new FrameworkName(".Net standard", new System.Version(2, 1)) },
                Take = int.MaxValue
            };

            var metaRequests = nuGetRepository.Search.Exec(filter);

            var convertedRequests = ConvertRequest(metaRequests);

            var enumer = new AsyncEnumerator<NuGetPackage>(convertedRequests);

            return AsyncEnumerable.FromResult(enumer);
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
