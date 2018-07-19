namespace Bars.NuGet.Querying.Client
{
    using global::Bars.NuGet.Querying.NuGet.Commands;
    using System;

    public partial class NuGetRepository : IDisposable
    {
        private SearchNuGetCommand searchNuGetCommand;
        internal SearchNuGetCommand Search
        {
            get
            {
                if (searchNuGetCommand == null)
                    searchNuGetCommand = new SearchNuGetCommand(this.Repositories, this.loggerAdapter);
                return searchNuGetCommand;
            }
        }
    }
}