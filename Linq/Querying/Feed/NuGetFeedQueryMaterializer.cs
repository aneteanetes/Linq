namespace Bars.NuGet.Querying.Feed
{
    using Bars.NuGet.Querying.Visitors;
    using global::Bars.NuGet.Querying.Client;
    using global::Bars.NuGet.Querying.Iterators;
    using global::Bars.NuGet.Querying.Types;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;

    internal class NuGetFeedQueryMaterializer
    {
        internal static IEnumerator<NuGetPackage> Execute(Expression expression, NuGetRepository nuGetRepository, Expression root)
        {
            var nuGetVisitor = new NuGetExpressionVisitor();
            nuGetVisitor.Visit(expression);

            var filter = nuGetVisitor.GetNuGetQueryFilter();

            var deffered = DefferedEnumerator(nuGetRepository, filter);

            if (filter.SyncIncompatibility)
            {
                return Synchronized(deffered, nuGetVisitor.GetNotEvaluated);
            }

            return deffered;
        }

        private static AsyncEnumerator<NuGetPackage> DefferedEnumerator(NuGetRepository nuGetRepository, NuGetQueryFilter filter)
        {
            var metaRequests = nuGetRepository.Search(filter);

            return new AsyncEnumerator<NuGetPackage>(metaRequests);
        }

        private static IEnumerator<NuGetPackage> Synchronized(AsyncEnumerator<NuGetPackage> async, Func<IQueryable<NuGetPackage>, IQueryable<NuGetPackage>> getNotEvaluated)
        {
            var syncList = new AsyncEnumerable<NuGetPackage>(async);

            var withFilter = getNotEvaluated(syncList.AsQueryable());

            return withFilter.GetEnumerator();
        }
    }
}