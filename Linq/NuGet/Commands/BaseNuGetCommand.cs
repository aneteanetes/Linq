namespace Bars.NuGet.Querying.Commands
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.Threading;
    using global::Bars.NuGet.Querying.Commands.CommandResult;
    using global::Bars.NuGet.Querying.Functionality;
    using global::NuGet.Common;
    using global::NuGet.Protocol.Core.Types;

    internal abstract class BaseNuGetCommand<T> where T : class, INuGetResource
    {
        protected IEnumerable<SourceRepository> sourceRepositories;
        protected ILogger nugetLogger;
        protected CancellationToken cancellationToken;

        protected BaseNuGetCommand()
        {

        }

        public BaseNuGetCommand(IEnumerable<SourceRepository> sourceRepositories, ILogger nugetLogger, CancellationToken cancellationToken)
        {
            this.nugetLogger = nugetLogger;
            this.sourceRepositories = sourceRepositories;
            this.cancellationToken = cancellationToken == default
                ? CancellationToken.None
                : cancellationToken;
        }

        protected NuGetCommandResult<T> Exec() => new NuGetCommandResult<T>(Resources);

        private IEnumerable<T> Resources() => Binder.Bind(this.sourceRepositories, ResourceGetter);

        private Func<SourceRepository, T> ResourceGetter = repo => repo.GetResource<T>();
        
    }
}
