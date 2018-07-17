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

    /// <summary>
    /// Репозиторий который агрегирует несколько NuGet-фидов или других репозиториев
    /// </summary>
    public partial class NuGetRepository : IDisposable
    {
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
        /// Адаптери логировщиков
        /// </summary>
        private readonly List<NuGetLoggerAdapter> loggerAdapters = new List<NuGetLoggerAdapter>();

        /// <summary>
        /// Создаёт новое объектное представление агрегированного репозитория.
        /// </summary>
        /// <param name="feeds">Список фидов, с авторизацией</param>
        public NuGetRepository(Microsoft.Extensions.Logging.ILogger logger)
        {
            this.loggerAdapters.Add(new NuGetLoggerAdapter(logger));
        }

        /// <summary>
        /// Создаёт новое объектное представление агрегированного репозитория.
        /// Так же, заполняет фреймворки: .NET standard с 1.0 до 2.1
        /// </summary>
        /// <param name="feeds">Список фидов, с авторизацией</param>
        public NuGetRepository(IEnumerable<string> feeds) : this(feeds, null)
        {
            (int major, int minor)[] versions = { (1, 0), (1, 1), (1, 2), (1, 3), (1, 4), (1, 5), (1, 6), (2, 0), (2, 1) };

            this.Frameworks = versions.Select(version => new NuGetFramework(".NETStandard", new Version(version.major, version.minor)));
        }

        /// <summary>
        /// Создаёт новое объектное представление агрегированного репозитория.
        /// </summary>
        /// <param name="feeds">Список фидов, с авторизацией</param>
        /// <param name="frameworks">Список фреймворков, для поиска пакетов для подходящей платформы</param>
        public NuGetRepository(IEnumerable<string> feeds, NuGetFramework[] frameworks) : this((Microsoft.Extensions.Logging.ILogger)null)
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
        public NuGetRepository(params NuGetRepository[] aggregateRepositories) : this((Microsoft.Extensions.Logging.ILogger)null)
        {
            var repositories = new List<SourceRepository>();
            var frameworks = new List<NuGetFramework>();            

            foreach (var aggregateRepository in aggregateRepositories)
            {
                repositories.AddRange(aggregateRepository.Repositories);
                loggerAdapters.AddRange(aggregateRepository.loggerAdapters);

                if (aggregateRepository.Frameworks != null)
                {
                    frameworks.AddRange(aggregateRepository.Frameworks);
                }
                frameworks = frameworks.Distinct().ToList();
            }

            this.Frameworks = frameworks;
            this.Repositories = repositories;
        }
        
        public IEnumerable<Microsoft.Extensions.Logging.ILogger> Loggers
        {
            get => loggerAdapters.Select(x => x.logger);
            set
            {
                this.loggerAdapters.AddRange(value.Select(l => new NuGetLoggerAdapter(l)));
            }
        }

        /// <summary>
        /// Здесь освобождается кэш
        /// </summary>
        public void Dispose()
        {
            this.sourceCacheContext.Dispose();
        }
    }
}