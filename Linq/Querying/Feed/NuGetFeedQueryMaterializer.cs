namespace Bars.NuGet.Querying.Feed
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using Bars.NuGet.Querying.Visitors;
    using global::Bars.NuGet.Querying.Client;
    using global::Bars.NuGet.Querying.Iterators;
    using global::Bars.NuGet.Querying.Types;

    internal class NuGetFeedQueryMaterializer
    {
        internal static IEnumerator<NuGetPackage> Execute(Expression expression, NuGetRepository nuGetRepository)
        {
            var nuGetVisitor = GetVisitor(expression);

            var filter = GetFilter(nuGetVisitor);

            var deffered = DefferedEnumerator(nuGetRepository, filter);

            if (!filter.SyncIncompatibility)
                return deffered;
            
            return Synchronized(deffered, nuGetVisitor.GetNotEvaluated);
        }

        private static NuGetExpressionVisitor GetVisitor(Expression expression)
        {
            var nuGetVisitor = new NuGetExpressionVisitor();
            nuGetVisitor.Visit(expression);
            return nuGetVisitor;
        }

        private static NuGetQueryFilter GetFilter(NuGetExpressionVisitor nuGetVisitor)
        {
            var filter = nuGetVisitor.GetNuGetQueryFilter();

            var validator = NuGetQueryValidator.ValidateFilter(filter);

            if (!validator.IsValid)
                throw new Exception(string.Join(Environment.NewLine, validator.Errors.Select(err => err.Message)));

            return filter;
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