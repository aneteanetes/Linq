namespace Bars.NuGet.Querying.NuGet.Commands
{
    using global::NuGet.Protocol.Core.Types;

    /// <summary>
    /// Accessor for nuget inner resources
    /// </summary>
    public sealed class NuGetResourceAccessor
    {
        private SourceRepository sourceRepository;

        /// <summary>
        /// Creates new nuget resource accessor
        /// </summary>
        /// <param name="sourceRepository">target repository</param>
        public NuGetResourceAccessor(SourceRepository sourceRepository)
        {
            this.sourceRepository = sourceRepository;
        }

        /// <summary>
        /// Get resource from repository, alternative for extension method
        /// </summary>
        /// <typeparam name="TResource"></typeparam>
        /// <returns></returns>
        public TResource GetResource<TResource>() where TResource : class, INuGetResource
        {
            return sourceRepository.GetResource<TResource>();
        }
    }
}