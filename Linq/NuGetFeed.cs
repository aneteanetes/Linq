namespace Bars.NuGet.Querying
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Threading.Tasks;
    using Bars.NuGet.Querying.Client;
    using Bars.NuGet.Querying.Iterators;
    using global::Bars.Linq.Async;
    using global::Bars.NuGet.Querying.Feed;
    using Microsoft.Extensions.Logging;

    public class NuGetFeed : IAsyncQueryable<NuGetPackage>
    {
        private NuGetRepository nuGetRepository;

        public NuGetFeed(string localDir, ILogger logger, params string[] feeds)
        {
            nuGetRepository = new NuGetRepository(localDir, feeds, logger);

            Expression = Expression.Constant(this);
            AsyncProvider = new NuGetFeedQueryProvider(nuGetRepository, Expression);
            Provider = AsyncProvider;
        }

        internal NuGetFeed(IQueryProvider provider, Expression expression)
        {
            var asyncQueryProvider = provider as IAsyncQueryProvider<NuGetPackage>;
            Provider = provider;
            AsyncProvider = asyncQueryProvider;
            Expression = expression;
        }

        public IEnumerator<NuGetPackage> GetEnumerator()
        {
            return Provider.Execute<IEnumerable<NuGetPackage>>(Expression).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();

        public IAsyncEnumerator<NuGetPackage> GetAsyncEnumerator()
        {
            return AsyncProvider.AsyncExecute(Expression).GetAsyncEnumerator();
        }

        public Type ElementType { get { return typeof(NuGetPackage); } }

        public Expression Expression { get; private set; }

        public IQueryProvider Provider { get; private set; }

        public IAsyncQueryProvider<NuGetPackage> AsyncProvider { get; private set; }

        /// <summary>
        /// add or update new package to all feeds
        /// тут нужны перегрузки для конкретного фида, либо отдельный фид комбайнить, тогда нужен злой конструктор фида как у нугет репоза
        /// </summary>
        /// <param name="nuGetPackage"></param>
        /// <returns></returns>
        public AsyncEnumerable<bool> Add(NuGetPackage nuGetPackage)
        {
            var enumerator = new AsyncEnumerator<bool>(this.nuGetRepository.Upload(nuGetPackage));
            var enumerable = AsyncEnumerable.FromResult(enumerator);            
            return enumerable;
        }

        /// <summary>
        /// remove package from all feeds
        /// тут нужны перегрузки для конкретного фида, либо отдельный фид комбайнить, тогда нужен злой конструктор фида как у нугет репоза
        /// </summary>
        /// <param name="nuGetPackage"></param>
        /// <returns></returns>
        public AsyncEnumerable<bool> Remove(NuGetPackage nuGetPackage)
        {
            var enumerator = new AsyncEnumerator<bool>(this.nuGetRepository.Remove(nuGetPackage));
            var enumerable = AsyncEnumerable.FromResult(enumerator);
            return enumerable;
        }
    }
}