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
        internal static IAsyncQueryable<NuGetPackage> Execute(Expression expression, NuGetRepository nuGetRepository)
        {
            var visitedExpression = new NuGetExpressionVisitor().Visit(expression);

            var queryableElements = Root(nuGetRepository, visitedExpression.filter);

            return queryableElements;
        }

        private static IAsyncQueryable<NuGetPackage> Root(NuGetRepository nuGetRepository, NuGetQueryFilter filter)
        {
            filter = new NuGetQueryFilter
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
