namespace Bars.NuGet.Querying.Logging
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using global::NuGet.Common;

    /// <summary>
    /// internal
    /// </summary>
    internal class NuGetLoggerAdapter : ILogger
    {
        public readonly Microsoft.Extensions.Logging.ILogger logger;

        public NuGetLoggerAdapter(Microsoft.Extensions.Logging.ILogger logger)
        {
            this.logger = logger;
            this.InitMap();
        }

        private readonly Dictionary<LogLevel, Action<Microsoft.Extensions.Logging.ILogger, Microsoft.Extensions.Logging.EventId, Exception, string, object[]>> map = new Dictionary<LogLevel, Action<Microsoft.Extensions.Logging.ILogger,Microsoft.Extensions.Logging.EventId, Exception, string, object[]>>();
        private void InitMap()
        {
            this.map.Add(LogLevel.Debug, Microsoft.Extensions.Logging.LoggerExtensions.LogDebug);
            this.map.Add(LogLevel.Error, Microsoft.Extensions.Logging.LoggerExtensions.LogError);
            this.map.Add(LogLevel.Information, Microsoft.Extensions.Logging.LoggerExtensions.LogInformation);
            this.map.Add(LogLevel.Minimal, Microsoft.Extensions.Logging.LoggerExtensions.LogTrace);
            this.map.Add(LogLevel.Verbose, Microsoft.Extensions.Logging.LoggerExtensions.LogTrace);
            this.map.Add(LogLevel.Warning, Microsoft.Extensions.Logging.LoggerExtensions.LogWarning);
        }

        public void Log(LogLevel level, string data)
        {
            this.map[level](
                this.logger,
                new Microsoft.Extensions.Logging.EventId(),
                null,
                data,
                new object[0]);
        }

        public Task LogAsync(LogLevel level, string data)
        {
            this.Log(level, data);
            return Task.CompletedTask;
        }

        public void Log(ILogMessage message) => this.Log(message.Level, message.Message);

        public Task LogAsync(ILogMessage message) => LogAsync(message.Level, message.Message);

        public void LogDebug(string data) => this.Log(LogLevel.Debug, data);

        public void LogVerbose(string data) => this.Log(LogLevel.Verbose, data);

        public void LogInformation(string data) => this.Log(LogLevel.Information, data);

        public void LogMinimal(string data) => this.Log(LogLevel.Minimal, data);

        public void LogWarning(string data) => this.Log(LogLevel.Warning, data);

        public void LogError(string data) => this.Log(LogLevel.Error, data);

        public void LogInformationSummary(string data) => this.Log(LogLevel.Information, data);
    }
}
