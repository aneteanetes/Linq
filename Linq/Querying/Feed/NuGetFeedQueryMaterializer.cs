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

            var queryableElements = Materialize(nuGetRepository, filter);

            if (filter.SyncIncompatibility)
            {
                return Synchronized(queryableElements, nuGetVisitor.GetNotEvaluated);
            }

            return queryableElements;
        }

        private static IAsyncQueryable<NuGetPackage> Materialize(NuGetRepository nuGetRepository, NuGetQueryFilter filter)
        {
            var metaRequests = nuGetRepository.Search(filter);

            var enumer = new AsyncEnumerator<NuGetPackage>(metaRequests);

            var enumerable = AsyncEnumerable.FromResult(enumer);

            return enumerable;
        }

        private static IAsyncQueryable<NuGetPackage> Synchronized(IAsyncQueryable<NuGetPackage> async, Func<IQueryable<NuGetPackage>, IQueryable<NuGetPackage>> getNotEvaluated)
        {
            var syncTask = AsyncEnumerableExtensions.ToList(async);
            syncTask.Wait();

            var completedList = syncTask.Result;

            var withFilter = getNotEvaluated(completedList.AsQueryable());

            var completedEnumer = new CompletedAsyncEnumerator<NuGetPackage>(withFilter);
            return AsyncEnumerable.FromResult(completedEnumer);
        }
    }
}