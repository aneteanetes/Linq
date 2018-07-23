namespace Bars.NuGet.Querying.NuGet.Commands
{
    using global::NuGet.Protocol.Core.Types;

    public sealed class NuGetResourceAccessor
    {
        private SourceRepository sourceRepository;

        public NuGetResourceAccessor(SourceRepository sourceRepository)
        {
            this.sourceRepository = sourceRepository;
        }

        internal TResource GetResource<TResource>() where TResource : class, INuGetResource
        {
            return sourceRepository.GetResource<TResource>();
        }
    }
}