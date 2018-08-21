namespace Bars.NuGet.Querying
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using Bars.NuGet.Querying.Client;
    using Bars.NuGet.Querying.Files;
    using Bars.NuGet.Querying.Iterators;
    using global::Bars.NuGet.Querying.Feed;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Logging.Abstractions;

    /// <summary>
    /// Queryable NuGet feed
    /// </summary>
    public class NuGetFeed : IOrderedQueryable<NuGetPackage>
    {
        private readonly NuGetRepository nuGetRepository;

        /// <summary>
        /// Initialize new feed with default path resolver in localDir
        /// </summary>
        /// <param name="localDir">absolute path to feed local path</param>
        /// <param name="feeds">collection of external feeds</param>
        public NuGetFeed(string localDir, params string[] feeds) 
            : this(localDir, NullLogger.Instance, feeds) { }

        /// <summary>
        /// Initialize new feed with path resolver and external feeds
        /// </summary>
        /// <param name="pathResolver">path resolver for nuget file unpack rules</param>
        /// <param name="feeds">collection of external feeds</param>
        public NuGetFeed(PathResolver pathResolver, params string[] feeds)
            : this(pathResolver, NullLogger.Instance, feeds) { }

        /// <summary>
        /// Initialize new feed with default path resolver in localDir
        /// </summary>
        /// <param name="localDir">absolute path to feed local path</param>
        /// <param name="logger">logger for nuget repository</param>
        /// <param name="feeds">collection of external feeds</param>
        public NuGetFeed(string localDir, ILogger logger, params string[] feeds)
            : this(new DefaultPathResolver { LocalRepositoryAbsolutePath = localDir }, logger, feeds) { }

        /// <summary>
        /// Initialize new feed with path resolver and external feeds
        /// </summary>
        /// <param name="pathResolver">path resolver for nuget file unpack rules</param>
        /// <param name="logger">logger for nuget repository</param>
        /// <param name="feeds">collection of external feeds</param>
        public NuGetFeed(PathResolver pathResolver, ILogger logger, params string[] feeds)
        {
            nuGetRepository = new NuGetRepository(pathResolver, feeds, logger);

            Expression = Expression.Constant(this);
            Provider = new NuGetFeedQueryProvider(nuGetRepository);
        }

        /// <summary>
        /// internal
        /// </summary>
        /// <param name="provider"></param>
        /// <param name="expression"></param>
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
        /// (feature)
        /// </summary>
        /// <param name="nuGetPackage"></param>
        /// <returns></returns>
        internal IEnumerable<bool> Add(NuGetPackage nuGetPackage)
        {
            var uploadTask = this.nuGetRepository.Upload(nuGetPackage);

            var enumerator = new AsyncEnumerator<bool>(uploadTask);

            return new AsyncEnumerable<bool>(enumerator);
        }

        /// <summary>
        /// remove package from all feeds
        /// (feature)
        /// </summary>
        /// <param name="nuGetPackage"></param>
        /// <returns></returns>
        internal IEnumerable<bool> Remove(NuGetPackage nuGetPackage)
        {
            var removeTask = this.nuGetRepository.Remove(nuGetPackage);

            var enumerator = new AsyncEnumerator<bool>(removeTask);

            return new AsyncEnumerable<bool>(enumerator);
        }
    }
}