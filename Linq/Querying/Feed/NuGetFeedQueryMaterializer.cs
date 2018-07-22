namespace Bars.NuGet.Querying.Feed
{
    using Bars.NuGet.Querying.Visitors;
    using global::Bars.Linq.Async;
    using global::Bars.NuGet.Querying.Client;
    using global::Bars.NuGet.Querying.Iterators;
    using global::Bars.NuGet.Querying.Types;
    using global::NuGet.Protocol.Core.Types;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Runtime.Versioning;
    using System.Threading.Tasks;

    internal class NuGetFeedQueryMaterializer
    {
        internal static IAsyncQueryable<NuGetPackage> Execute(Expression expression, NuGetRepository nuGetRepository, Expression root)
        {
            var nuGetVisitor = new NuGetExpressionVisitor();

            var visitedExpression = nuGetVisitor.Visit(expression);
            var filter = nuGetVisitor.GetNuGetQueryFilter();

            var queryableElements = Root(nuGetRepository, filter);

            if (filter.SyncIncompatibility)
            {
                return Synchronized(queryableElements, nuGetVisitor.GetNotEvaluated);
            }

            return queryableElements;
        }

        private static IAsyncQueryable<NuGetPackage> Root(NuGetRepository nuGetRepository, NuGetQueryFilter filter)
        {
            var metaRequests = nuGetRepository.Search.Exec(filter);

            var convertedRequests = ConvertRequest(metaRequests);

            var enumer = new AsyncEnumerator<NuGetPackage>(convertedRequests);

            var enumerable = AsyncEnumerable.FromResult(enumer);

            return enumerable;
        }

        private static IAsyncQueryable<NuGetPackage> Synchronized(IAsyncQueryable<NuGetPackage> async, Func<IQueryable<NuGetPackage>,IQueryable<NuGetPackage>> getNotEvaluated)
        {
            var syncTask = AsyncEnumerableExtensions.ToList(async);
            syncTask.Wait();

            var completedList = syncTask.Result;

            var withFilter = getNotEvaluated(completedList.AsQueryable());

            var completedEnumer = new CompletedAsyncEnumerator<NuGetPackage>(withFilter);
            return AsyncEnumerable.FromResult(completedEnumer);
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
                Id = meta.Identity.Id,
                Author = Guid.NewGuid().ToString().Substring(0,5),
                Owner = Guid.NewGuid().ToString().Substring(0, 5)
            };
        }
    }
}