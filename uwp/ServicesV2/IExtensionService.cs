using SoundByte.App.Uwp.Extensions.Core;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Windows.Storage;

namespace SoundByte.App.Uwp.ServicesV2
{
    /// <summary>
    ///     Service used to load and manage app extensions.
    /// </summary>
    public interface IExtensionService
    {
        /// <summary>
        ///     A list of extensions installed in this application.
        /// </summary>
        ObservableCollection<Extension> Extensions { get; }

        /// <summary>
        ///     The API version that SoundByte implements
        /// </summary>
        double ApiVersion { get; }

        /// <summary>
        ///     Load an extension into memory so it can be used
        ///     by the application.
        /// </summary>
        /// <param name="extensionLocation">
        ///     The folder where this extension is located. Should
        ///     contain the manifest.json file.
        /// </param>
        Task LoadExtensionAsync(StorageFolder extensionLocation, string libraryScript);

        /// <summary>
        ///     Install the extension by copying it the app data
        ///     folder so it's auto loaded on app startup.
        /// </summary>
        /// <param name="extensionLocation">
        ///     The folder where this extension is located. Should
        ///     contain the manifest.json file.
        /// </param>
        Task InstallExtensionAsync(StorageFolder extensionLocation, string libraryScript);

        /// <summary>
        ///     Finds all built in and installed extensions
        ///     and loads them into the application.
        /// </summary>
        Task FindAndLoadExtensionsAsync();
    }
}