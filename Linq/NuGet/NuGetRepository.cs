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
    /// Репозиторий который агрегирует несколько NuGet-фидов или других репозиториев
    /// </summary>
    public partial class NuGetRepository : IDisposable
    {
        private PathResolver pathResolver; 

        /// <summary>
        /// Репозитории-объекты из библиотеки NuGet
        /// </summary>
        private readonly IEnumerable<SourceRepository> Repositories;

        /// <summary>
        /// Фреймворки по которым происходит поиск пакетов
        /// </summary>
        private readonly IEnumerable<NuGetFramework> Frameworks;

        /// <summary>
        /// Кэш для всех операций
        /// </summary>
        private readonly SourceCacheContext sourceCacheContext = new SourceCacheContext();

        /// <summary>
        /// Адаптеры логировщиков
        /// </summary>
        private readonly NuGetLoggerAdapter loggerAdapter;

        /// <summary>
        /// Создаёт новое объектное представление агрегированного репозитория.
        /// </summary>
        /// <param name="feeds">Список фидов, с авторизацией</param>
        public NuGetRepository(string localDir, Microsoft.Extensions.Logging.ILogger logger)
        {
            this.pathResolver = new DefaultPathResolver { LocalRepositoryAbsolutePath = localDir };
            this.loggerAdapter = new NuGetLoggerAdapter(logger);
        }

        /// <summary>
        /// Создаёт новое объектное представление агрегированного репозитория.
        /// </summary>
        /// <param name="feeds">Список фидов, с авторизацией</param>
        public NuGetRepository(PathResolver pathResolver, Microsoft.Extensions.Logging.ILogger logger)
        {
            this.pathResolver = pathResolver;
            this.loggerAdapter = new NuGetLoggerAdapter(logger);
        }

        /// <summary>
        /// Создаёт новое объектное представление агрегированного репозитория.
        /// Так же, заполняет фреймворки: .NET standard с 1.0 до 2.1
        /// </summary>
        /// <param name="feeds">Список фидов, с авторизацией</param>
        public NuGetRepository(string localDir, IEnumerable<string> feeds) : this(localDir, feeds, null)
        {
        }

        /// <summary>
        /// Создаёт новое объектное представление агрегированного репозитория.
        /// </summary>
        /// <param name="feeds">Список фидов, с авторизацией</param>
        /// <param name="frameworks">Список фреймворков, для поиска пакетов для подходящей платформы</param>
        public NuGetRepository(string localDir, IEnumerable<string> feeds, Microsoft.Extensions.Logging.ILogger logger) : this(localDir, logger)
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
        /// Создаёт новое объектное представление агрегированного репозитория.
        /// </summary>
        /// <param name="aggregateRepositories">Другие агрегрированные репозитории</param>
        public NuGetRepository(string localDir, Microsoft.Extensions.Logging.ILogger logger, params NuGetRepository[] aggregateRepositories) : this(localDir, logger)
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

        public Microsoft.Extensions.Logging.ILogger Logger => this.loggerAdapter.logger;

        /// <summary>
        /// Здесь освобождается кэш
        /// </summary>
        public void Dispose()
        {
            this.sourceCacheContext.Dispose();
        }
    }
}