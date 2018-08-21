namespace Bars.NuGet.Querying
{
    using Bars.NuGet.Querying.Client;
    using Bars.NuGet.Querying.Iterators;
    using global::Bars.NuGet.Querying.Feed;
    using Microsoft.Extensions.Logging;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;

    public class NuGetFeed : IOrderedQueryable<NuGetPackage>
    {
        private readonly NuGetRepository nuGetRepository;

        public NuGetFeed(string localDir, ILogger logger, params string[] feeds)
        {
            nuGetRepository = new NuGetRepository(localDir, feeds, logger);

            Expression = Expression.Constant(this);
            Provider = new NuGetFeedQueryProvider(nuGetRepository);
        }

        internal NuGetFeed(IQueryProvider provider, Expression expression)
        {
            Provider = provider;
            Expression = expression;
        }

        public IEnumerator<NuGetPackage> GetEnumerator()
        {
            return Provider.Execute<IEnumerator<NuGetPackage>>(Expression);
        }

        IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();

        public Type ElementType { get { return typeof(NuGetPackage); } }

        public Expression Expression { get; private set; }

        public IQueryProvider Provider { get; private set; }

        /// <summary>
        /// add or update new package to all feeds
        /// тут нужны перегрузки для конкретного фида, либо отдельный фид комбайнить, тогда нужен злой конструктор фида как у нугет репоза
        /// </summary>
        /// <param name="nuGetPackage"></param>
        /// <returns></returns>
        public IEnumerable<bool> Add(NuGetPackage nuGetPackage)
        {
            var uploadTask = this.nuGetRepository.Upload(nuGetPackage);

            var enumerator = new AsyncEnumerator<bool>(uploadTask);

            return new AsyncEnumerable<bool>(enumerator);
        }

        /// <summary>
        /// remove package from all feeds
        /// тут нужны перегрузки для конкретного фида, либо отдельный фид комбайнить, тогда нужен злой конструктор фида как у нугет репоза
        /// </summary>
        /// <param name="nuGetPackage"></param>
        /// <returns></returns>
        public IEnumerable<bool> Remove(NuGetPackage nuGetPackage)
        {
            var removeTask = this.nuGetRepository.Remove(nuGetPackage);

            var enumerator = new AsyncEnumerator<bool>(removeTask);

            return new AsyncEnumerable<bool>(enumerator);
        }
    }
}