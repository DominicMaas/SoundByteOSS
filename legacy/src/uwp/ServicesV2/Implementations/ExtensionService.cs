using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Toolkit.Uwp.Helpers;
using SoundByte.App.Uwp.Extensions.Core;
using SoundByte.App.Uwp.Extensions.Exceptions;
using Windows.ApplicationModel;
using Windows.Storage;
using WinRTXamlToolkit.IO.Extensions;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace SoundByte.App.Uwp.ServicesV2.Implementations
{
    public class ExtensionService : IExtensionService
    {
        public ObservableCollection<Extension> Extensions { get; } = new ObservableCollection<Extension>();

        public double ApiVersion => 1.0;

        public async Task FindAndLoadExtensionsAsync()
        {
            string libraryScript;
            try
            {
                // Load the JavaScript library
                var libraryDirectory = await Package.Current.InstalledLocation.GetFolderAsync(@"Assets\ExtensionLibs");
                var sourceCodeFile = await libraryDirectory.GetFileByPathAsync("soundbyte-lib.js");
                using var sourceStream = await sourceCodeFile.OpenStreamForReadAsync();
                using var streamReader = new StreamReader(sourceStream);
                libraryScript = await streamReader.ReadToEndAsync();
            }
            catch (Exception ex)
            {
                App.NotificationManager?.Show("Error loading SoundByte platform " + ex.Message);
                return;
            }

            try
            {
                var coreExtensionsDirectory = await Package.Current.InstalledLocation.GetFolderAsync(@"Assets\CoreExtensions");
                foreach (var folder in await coreExtensionsDirectory.GetFoldersAsync())
                {
                    var innerFolder = await folder.GetFolderAsync("extension");
                    await LoadExtensionAsync(innerFolder, libraryScript);
                }
            }
            catch (Exception ex)
            {
                App.NotificationManager?.Show("Error loading core extensions: " + ex.Message);
            }

            try
            {
                var customExtensionDirectory = await ApplicationData.Current.LocalFolder.CreateFolderAsync("extensions", CreationCollisionOption.OpenIfExists);
                foreach (var folder in await customExtensionDirectory.GetFoldersAsync())
                {
                    await LoadExtensionAsync(folder, libraryScript);
                }
            }
            catch (Exception ex)
            {
                App.NotificationManager?.Show("Error loading custom extensions: " + ex.Message);
            }
        }

        public Task InstallExtensionAsync(StorageFolder extensionLocation, string libraryScript)
        {
            return LoadExtensionAsync(extensionLocation, libraryScript);
        }

        public async Task LoadExtensionAsync(StorageFolder extensionLocation, string libraryScript)
        {
            // Check if the manifest exists in the root location, if not, throw
            // an exception.
            var doesManifestExist = await extensionLocation.FileExistsAsync("manifest.yaml");
            if (!doesManifestExist)
                throw new ManifestInvalidException(InvalidManifestError.Missing);

            // Temp class that we will deserialize into
            ExtensionManifest extensionManifest;

            try
            {
                // Load the manifest stream and deserialize the contents
                using var manifestStream = await extensionLocation.OpenStreamForReadAsync("manifest.yaml");
                using var streamReader = new StreamReader(manifestStream);

                // Deserialize and set the manifest class
                var deserializer = new DeserializerBuilder()
                    .WithNamingConvention(new CamelCaseNamingConvention())
                    .Build();

                extensionManifest = deserializer.Deserialize<ExtensionManifest>(streamReader);
            }
            catch (Exception ex)
            {
                // An error occurred while reading the manifest, the manifest is invalid
                throw new ManifestInvalidException(InvalidManifestError.Invalid, ex);
            }

            // Where we will store the script once loaded from the file
            string script;

            // Check to see if the user has provided a script
            if (string.IsNullOrEmpty(extensionManifest.Script))
                throw new ManifestInvalidException(InvalidManifestError.MissingScript);

            // Try load the source code file
            try
            {
                var sourceCodeFile = await extensionLocation.GetFileByPathAsync(extensionManifest.Script);

                using var sourceStream = await sourceCodeFile.OpenStreamForReadAsync();
                using var streamReader = new StreamReader(sourceStream);

                script = await streamReader.ReadToEndAsync();
            }
            catch (Exception ex)
            {
                // The script is missing or unreadable
                throw new ManifestInvalidException(InvalidManifestError.MissingScript, ex);
            }

            // Create the extension and validate the rest of the manifest file
            var extension = new Extension(extensionLocation, extensionManifest, true, script, libraryScript);

            // Add to list
            Extensions.Add(extension);
        }
    }
}