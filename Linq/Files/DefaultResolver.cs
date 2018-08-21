namespace Bars.NuGet.Querying.Files
{
    using System.IO;

    /// <summary>
    /// internal default path resolver
    /// </summary>
    internal class DefaultPathResolver : PathResolver
    {
        public override string ResolveFilePath(NuGetPackage nuGetPackage)
        {
            return Path.Combine(this.LocalRepositoryAbsolutePath, nuGetPackage.Id, nuGetPackage.Version.ToString(), $"{nuGetPackage.Id}.nupkg");
        }
    }
}