
namespace BarsUp.Designer.Workspace.NuGet.Utility
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public partial class AggregateRepository
    {
        private readonly global::NuGet.Common.ILogger logger;

        /// <summary>
        /// Создаёт новое объектное представление агрегированного репозитория с логером
        /// </summary>
        public AggregateRepository()
        {
            this.logger = new AggregateRepositoryLogger();            
        }

        /// <summary>
        /// Приватный прокси-класс для логирования
        /// </summary>
        private class AggregateRepositoryLogger : global::NuGet.Common.LoggerBase
        {
            /// <summary>
            /// Главный логгер приложения
            /// </summary>
            private readonly Serilog.ILogger trulyLogger = Serilog.Log.Logger;
            
            /// <summary>
            /// Шаблон для лога
            /// </summary>
            private const string tmpl = "NuGet: {Message}, project: {ProjectPath}, code: {Code}, when: {Time}, warn:{WarningLevel}";

            private readonly Dictionary<global::NuGet.Common.LogLevel, Action<string, object[]>> Map = new Dictionary<global::NuGet.Common.LogLevel, Action<string, object[]>>();

            public AggregateRepositoryLogger()
            {
                Map.Add(global::NuGet.Common.LogLevel.Debug, trulyLogger.Debug);
                Map.Add(global::NuGet.Common.LogLevel.Verbose, trulyLogger.Verbose);
                Map.Add(global::NuGet.Common.LogLevel.Minimal, trulyLogger.Information);
                Map.Add(global::NuGet.Common.LogLevel.Information, trulyLogger.Information);
                Map.Add(global::NuGet.Common.LogLevel.Warning, trulyLogger.Warning);
                Map.Add(global::NuGet.Common.LogLevel.Error, trulyLogger.Error);
            }

            public override void Log(global::NuGet.Common.ILogMessage message)
            {
                var objs = new object[]
                {
                    message.Message,
                    message.ProjectPath,
                    message.Code,
                    message.Time,
                    message.WarningLevel
                };

                Map[message.Level](tmpl, objs);
            }

            public override Task LogAsync(global::NuGet.Common.ILogMessage message)
            {
                Log(message);
                return Task.CompletedTask;
            }
        }
    }
}