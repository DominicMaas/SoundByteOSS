using SoundByte.Core.Models.MusicProvider;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace SoundByte.Core.Services.Definitions
{
    /// <summary>
    ///     Service used for managing music providers within SoundByte
    /// </summary>
    public interface IMusicProviderService
    {
        /// <summary>
        ///     A list of music providers installed in this application.
        /// </summary>
        ObservableCollection<MusicProvider> MusicProviders { get; }

        /// <summary>
        ///     The API version that SoundByte implements
        /// </summary>
        double ApiVersion { get; }

        /// <summary>
        ///     Installs a music provider from the SoundByte Store
        /// </summary>
        /// <param name="id">The music provider id to install</param>
        Task InstallAsync(Guid id);

        /// <summary>
        ///     Uninstalls a music provider which is currently installed
        /// </summary>
        /// <param name="id">The music provider id to uninstall</param>
        Task UninstallAsync(Guid id);

        /// <summary>
        ///     Load a music provider from the file system
        /// </summary>
        /// <param name="manifestPath">The path of where the manifest is located</param>
        Task LoadAsync(string manifestPath);

        /// <summary>
        ///     Loops through the installed music providers, checks for updates and if there are
        ///     any updates, updates the music provider.
        /// </summary>
        /// <returns></returns>
        Task CheckForUpdatesAndInstallAsync();

        /// <summary>
        ///     Finds all installed music providers
        ///     and loads them into the application.
        /// </summary>
        Task FindAndLoadAsync();
    }
}