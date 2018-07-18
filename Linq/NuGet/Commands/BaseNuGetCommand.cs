namespace Bars.NuGet.Querying.Commands
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using global::Bars.NuGet.Querying.Functionality;
    using global::NuGet.Common;
    using global::NuGet.Protocol.Core.Types;

    internal abstract class BaseNuGetCommand<TResult,TParam, TCommand> where TCommand : class, INuGetResource
    {
        protected IEnumerable<SourceRepository> sourceRepositories;
        protected ILogger nugetLogger;
        protected CancellationToken cancellationToken = CancellationToken.None;
        
        public BaseNuGetCommand(IEnumerable<SourceRepository> sourceRepositories, ILogger nugetLogger)
        {
            this.nugetLogger = nugetLogger;
            this.sourceRepositories = sourceRepositories;
        }
        
        private IEnumerable<TCommand> Resources() => Binder.Bind(this.sourceRepositories, ResourceGetter);

        private Func<SourceRepository, TCommand> ResourceGetter = repo => repo.GetResource<TCommand>();

        protected abstract Task<IEnumerable<TResult>> Command(TParam param, TCommand command);

        public IEnumerable<Task<IEnumerable<TResult>>> Exec(TParam param)
        {
            var commandResult = new NuGetCommandResult<TCommand>(Resources);
            return commandResult.Call(resource => Command(param, resource));
        }
    }
}
