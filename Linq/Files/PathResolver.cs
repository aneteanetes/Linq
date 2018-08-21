namespace Bars.NuGet.Querying.Files
{
    public abstract class PathResolver
    {
        /// <summary>
        /// Absolute file path for local repository, nuget can work only with local files
        /// </summary>
        public string LocalRepositoryAbsolutePath { get; set; }

        /// <summary>
        /// Return absolute path for *.nupkg file. Other files will be unpacked with *.nupkg folder hierarchy
        /// </summary>
        /// <param name="nuGetPackage"></param>
        /// <returns></returns>
        public abstract string ResolveFilePath(NuGetPackage nuGetPackage);
    }
}