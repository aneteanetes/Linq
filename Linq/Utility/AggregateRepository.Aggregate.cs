
namespace BarsUp.Designer.Workspace.NuGet.Utility
{
    using global::NuGet.Configuration;
    using global::NuGet.Frameworks;
    using global::NuGet.Protocol;
    using global::NuGet.Protocol.Core.Types;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// Репозиторий который агрегирует несколько NuGet-фидов или других репозиториев
    /// </summary>
    public partial class AggregateRepository : IDisposable
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
        /// Создаёт новое объектное представление агрегированного репозитория.
        /// Так же, заполняет фреймворки: .NET standard с 1.0 до 2.1
        /// </summary>
        /// <param name="feeds">Список фидов, с авторизацией</param>
        public AggregateRepository(IEnumerable<string> feeds) : this(feeds, null)
        {
            (int major, int minor)[] versions = { (1, 0), (1, 1), (1, 2), (1, 3), (1, 4), (1, 5), (1, 6), (2, 0), (2, 1) };

            this.Frameworks = versions.Select(version => new NuGetFramework(".NETStandard", new Version(version.major, version.minor)));
        }

        /// <summary>
        /// Создаёт новое объектное представление агрегированного репозитория.
        /// </summary>
        /// <param name="feeds">Список фидов, с авторизацией</param>
        /// <param name="frameworks">Список фреймворков, для поиска пакетов для подходящей платформы</param>
        public AggregateRepository(IEnumerable<string> feeds, NuGetFramework[] frameworks) : this()
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
        public AggregateRepository(params AggregateRepository[] aggregateRepositories) : this()
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
            }

            this.Frameworks = frameworks;
            this.Repositories = repositories;
        }

        /// <summary>
        /// Вызвать операцию для всех репозиториев
        /// </summary>
        /// <typeparam name="TResource">Ресурс который планируется использовать</typeparam>
        /// <typeparam name="TResult">Тип результата полученный после выполнения операции</typeparam>
        /// <param name="aggregatedCommand">Функтор использующий ресурс</param>
        /// <returns>Результат выполнения функтора</returns>
        protected IEnumerable<TResult> Exec<TResource, TResult>(Func<TResource, TResult> aggregatedCommand) where TResource : class, INuGetResource
        {
            var result = new List<TResult>();

            foreach (var repository in Repositories)
            {
                var commandResult = aggregatedCommand(repository.GetResource<TResource>());
                result.Add(commandResult);
            }

            return result;
        }

        /// <summary>
        /// Вызвать операцию для всех репозиториев, а затем агрегировать по функтору
        /// </summary>
        /// <typeparam name="TResource">Ресурс который планируется использовать</typeparam>
        /// <typeparam name="TResult">Тип результата полученный после выполнения операции</typeparam>
        /// <param name="aggregatedCommand">Функтор использующий ресурс</param>
        /// <param name="splitter">Функтор агрегирующий результаты</param>
        /// <returns>Результат выполнения функтора</returns>
        protected TResult ExecSplit<TResource, TResult>(Func<TResource, TResult> aggregatedCommand, Func<IEnumerable<TResult>, TResult> splitter) where TResource : class, INuGetResource
        {
            return splitter(Exec(aggregatedCommand));
        }

        /// <summary>
        /// Вызвать операцию для всех репозиториев, а затем агрегировать по функтору
        /// </summary>
        /// <typeparam name="TResource">Ресурс который планируется использовать</typeparam>
        /// <typeparam name="TResult">Тип результата полученный после выполнения операции</typeparam>
        /// <param name="aggregatedCommand">Функтор использующий ресурс, и передающий информацию об источнике пакетов</param>
        /// <returns>Результат выполнения функтора</returns>
        protected IEnumerable<TResult> Exec<TResource, TResult>(Func<TResource, PackageSource, TResult> aggregatedCommand) where TResource : class, INuGetResource
        {
            var result = new List<TResult>();

            foreach (var repository in Repositories)
            {
                var commandResult = aggregatedCommand(repository.GetResource<TResource>(), repository.PackageSource);
                result.Add(commandResult);
            }

            return result;
        }


        /// <summary>
        /// Вызвать операцию для всех репозиториев, а затем агрегировать по функтору
        /// </summary>
        /// <typeparam name="TResource">Ресурс который планируется использовать</typeparam>
        /// <typeparam name="TResult">Тип результата полученный после выполнения операции</typeparam>
        /// <param name="aggregatedCommand">Функтор использующий ресурс, и передающий информацию об источнике пакетов</param>
        /// <param name="splitter">Функтор агрегирующий результаты</param>
        /// <returns>Результат выполнения функтора</returns>
        protected TResult ExecSplit<TResource, TResult>(Func<TResource, PackageSource, TResult> aggregatedCommand, Func<IEnumerable<TResult>, TResult> splitter) where TResource : class, INuGetResource
        {
            return splitter(Exec(aggregatedCommand));
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