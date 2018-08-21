namespace Bars.NuGet.Querying.Client
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using global::NuGet.Configuration;
    using global::NuGet.Frameworks;
    using global::NuGet.Protocol;
    using global::NuGet.Protocol.Core.Types;
    using global::Bars.NuGet.Querying.Logging;
    using Bars.NuGet.Querying.Files;

    /// <summary>
    /// Aggregate repository for multiple nuget feeds or another repositories
    /// </summary>
    public partial class NuGetRepository : IDisposable
    {
        /// <summary>
        /// Current logger instance
        /// </summary>
        public Microsoft.Extensions.Logging.ILogger Logger => this.loggerAdapter.logger;

        /// <summary>
        /// inner resolver
        /// </summary>
        private PathResolver pathResolver; 

        /// <summary>
        /// Native nuget repositories
        /// </summary>
        private readonly IEnumerable<SourceRepository> Repositories;

        /// <summary>
        /// Target frameworks
        /// </summary>
        private readonly IEnumerable<NuGetFramework> Frameworks;

        /// <summary>
        /// One cache for all repositories
        /// Кэш для всех операций
        /// </summary>
        private readonly SourceCacheContext sourceCacheContext = new SourceCacheContext();

        /// <summary>
        /// Adapter for nuget.logger to microsoft.ext.logging
        /// </summary>
        private readonly NuGetLoggerAdapter loggerAdapter;

        /// <summary>        
        /// Creates new aggregated repository
        /// </summary>
        /// <param name="pathResolver">path resolver</param>
        /// <param name="logger">logger</param>
        public NuGetRepository(PathResolver pathResolver, Microsoft.Extensions.Logging.ILogger logger)
        {
            this.pathResolver = pathResolver;
            this.loggerAdapter = new NuGetLoggerAdapter(logger);
        }

        /// <summary>
        /// Creates new aggregated repository
        /// </summary>
        /// <param name="pathResolver">path resolver</param>
        /// <param name="feeds">feeds</param>
        public NuGetRepository(PathResolver resolver, IEnumerable<string> feeds) : this(resolver, feeds, null)
        {
        }

        /// <summary>
        /// Creates new aggregated repository
        /// </summary>
        /// <param name="pathResolver">path resolver</param>
        /// <param name="feeds">feeds</param>
        /// <param name="logger">logger</param>
        public NuGetRepository(PathResolver resolver, IEnumerable<string> feeds, Microsoft.Extensions.Logging.ILogger logger) : this(resolver, logger)
        {
            var repositories = new List<SourceRepository>();

            foreach (var feed in feeds)
            {
                var isV3 = feed.EndsWith(".json", StringComparison.InvariantCultureIgnoreCase);

                if (isV3)
                {
                    repositories.Add(Repository.Factory.GetCoreV3(feed));
                }
                else
                {
                    var packageSourceV2 = new PackageSource(feed);
                    repositories.Add(Repository.Factory.GetCoreV2(packageSourceV2));
                }
            }

            Repositories = repositories.ToArray();
        }

        /// <summary>
        /// Creates new aggregated repository
        /// </summary>
        /// <param name="pathResolver">path resolver</param>
        /// <param name="feeds">feeds</param>
        /// <param name="aggregateRepositories">another repositories</param>
        public NuGetRepository(PathResolver resolver, Microsoft.Extensions.Logging.ILogger logger, params NuGetRepository[] aggregateRepositories) : this(resolver, logger)
        {
            var repositories = new List<SourceRepository>();
            var frameworks = new List<NuGetFramework>();

            foreach (var aggregateRepository in aggregateRepositories)
            {
                repositories.AddRange(aggregateRepository.Repositories);

                if (aggregateRepository.Frameworks != null)
                {
                    frameworks.AddRange(aggregateRepository.Frameworks);
                }
                frameworks = frameworks.Distinct().ToList();
            }

            this.Frameworks = frameworks;
            this.Repositories = repositories;
        }

        /// <summary>
        /// dispose cache
        /// </summary>
        public void Dispose()
        {
            this.sourceCacheContext.Dispose();
        }
    }
}