using Flurl;
using Flurl.Http;
using Microsoft.AppCenter.Crashes;
using MvvmCross.Navigation;
using SoundByte.Core.Exceptions;
using SoundByte.Core.Models.MusicProvider;
using SoundByte.Core.Services.Definitions;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MvvmCross.Logging;
using Xamarin.Essentials;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace SoundByte.Core.Services.Implementations
{
    public class MusicProviderService : IMusicProviderService
    {
        private readonly IDialogService _dialogService;
        private readonly ISettingsService _settingsService;
        private readonly IMvxNavigationService _mvxNavigationService;
        private readonly IContentService _contentService;
        private readonly IAuthenticationService _authenticationService;
        private readonly IStoreService _storeService;
        private readonly IMvxLogProvider _logProvider;
        private readonly IMvxLog _log;

        private static readonly string MusicProviderPath = Path.Combine(FileSystem.AppDataDirectory, "music-providers");

        public MusicProviderService(IDialogService dialogService, IMvxNavigationService mvxNavigationService, ISettingsService settingsService,
            IContentService contentService, IAuthenticationService authenticationService, IStoreService storeService, IMvxLogProvider logProvider)
        {
            _dialogService = dialogService;
            _settingsService = settingsService;
            _mvxNavigationService = mvxNavigationService;
            _contentService = contentService;
            _authenticationService = authenticationService;
            _storeService = storeService;
            _logProvider = logProvider;
            _log = logProvider.GetLogFor<MusicProviderService>();
        }

        public ObservableCollection<MusicProvider> MusicProviders { get; } = new ObservableCollection<MusicProvider>();

        public double ApiVersion => 1.0;

        public Task CheckForUpdatesAndInstallAsync()
        {
            return Task.CompletedTask;
        }

        public async Task FindAndLoadAsync()
        {
            var errorBuilder = new StringBuilder();

            // Ensure the directory exists
            Directory.CreateDirectory(MusicProviderPath);

            // Loop through the music provider folders
            var musicProviderFolders = Directory.EnumerateDirectories(MusicProviderPath);
            foreach (var musicProviderFolder in musicProviderFolders)
            {
                try
                {
                    ReadMusicProvider(musicProviderFolder, true);
                }
                catch (Exception ex)
                {
                    Directory.Delete(Path.Combine(musicProviderFolder), true);
                    errorBuilder.AppendLine($"• {ex.Message} : {ex.InnerException?.Message ?? "Error"}");
                }
            }

            if (errorBuilder.Length > 0)
            {
                await _dialogService.ShowErrorMessageAsync("Error loading one or more music providers", errorBuilder.ToString());
            }
        }

        public async Task InstallAsync(Guid id)
        {
            // Ensure the user has premium if installing more than 5 providers
            var hasPremium = await _storeService.HasPremiumAsync();
            if (MusicProviders.Count >= 5 && !hasPremium)
            {
                await _dialogService.ShowInfoMessageAsync("Premium Required", "The free version of SoundByte only allows 5 music providers to be installed at once. Either uninstall a music provider, or upgrade the app.");
                return;
            }

            try
            {
                // Get the install response
                var response = await Constants.SoundByteMediaWebsite
                    .AppendPathSegment($"/api/v1/music-providers/install/{id}")
                    .SetQueryParams(new
                    {
                        max_api_version = ApiVersion
                    })
                    .WithHeader("User-Agent", "SoundByte App")
                    .GetJsonAsync<InstallResult>();

                // Download to temp
                var tempZipFile = await response.DownloadUrl.DownloadFileAsync(Path.GetTempPath(), Guid.NewGuid().ToString());

                // Extract
                var tempExtractPath = Path.Combine(Path.GetTempPath(), Guid.NewGuid() + "-extracted");
                ZipFile.ExtractToDirectory(tempZipFile, tempExtractPath);

                // Read the provider
                ReadMusicProvider(tempExtractPath, false);

                // Delete temp files
                File.Delete(tempZipFile);

                // Give the user some feedback
                var p = MusicProviders.FirstOrDefault(x => x.Identifier == id);
                if (p != null)
                {
                    await _dialogService.ShowInfoMessageAsync("Installation Successful", p.Manifest.Name + " has been installed and enabled");
                }
            }
            catch (FlurlHttpException ex)
            {
                var reason = await ex.GetResponseStringAsync();
                await _dialogService.ShowErrorMessageAsync("Could not install music provider", reason);
            }
            catch (Exception e)
            {
                await _dialogService.ShowErrorMessageAsync("Could not install music provider", e.Message);
            }
        }

        public async Task UninstallAsync(Guid id)
        {
            var musicProvider = MusicProviders.FirstOrDefault(x => x.Identifier == id);
            if (musicProvider == null)
                throw new Exception("A music provider with this id is not currently installed");

            // Remove content
            _contentService.RemoveContentByMusicProvider(id);

            // Logout (if logged in)
            await _authenticationService.DisconnectAccountAsync(id);

            // Remove from memory
            MusicProviders.Remove(musicProvider);

            // Delete installation folder (prevent reloading on startup)
            Directory.Delete(Path.Combine(MusicProviderPath, id.ToString()), true);
        }

        public async Task LoadAsync(string manifestPath)
        {
            try
            {
                // Ensure the directory exists
                if (!Directory.Exists(manifestPath))
                    throw new DirectoryNotFoundException("The provided directory was not found");

                ReadMusicProvider(manifestPath, false);
            }
            catch (Exception ex)
            {
                _log.ErrorException("Could not load music provider", ex);
                Crashes.TrackError(ex);
                await _dialogService.ShowErrorMessageAsync("Error loading music provider", ex.Message);
            }
        }

        private void ReadMusicProvider(string path, bool packed)
        {
            // Check if the manifest exists in the root location, if not, throw
            // an exception.
            var manifestLocation = Path.Combine(path, "manifest.yml");
            if (!File.Exists(manifestLocation))
                throw new ManifestInvalidException(InvalidManifestReason.Missing);

            // Temp class that we will deserialize into
            Manifest manifest;

            try
            {
                // Load the manifest stream and deserialize the contents
                using var manifestStream = File.OpenRead(manifestLocation);
                using var manifestStreamReader = new StreamReader(manifestStream);

                // Deserialize and set the manifest class
                var deserializer = new DeserializerBuilder()
                    .IgnoreUnmatchedProperties()
                    .WithNamingConvention(CamelCaseNamingConvention.Instance)
                    .Build();

                manifest = deserializer.Deserialize<Manifest>(manifestStreamReader);
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex);
                // An error occurred while reading the manifest, the manifest is invalid
                throw new ManifestInvalidException(InvalidManifestReason.Invalid, ex);
            }

            // If the music provider is already installed, we need to update it, this pretty much
            // means we uninstall the extension, but don't log the user out
            var existingProvider = MusicProviders.FirstOrDefault(x => x.Identifier == manifest.Id);
            if (existingProvider != null)
            {
                // Delete installation folder (will be overridden with the new provider version)
                Directory.Delete(Path.Combine(MusicProviderPath, existingProvider.Identifier.ToString()), true);

                // Remove content and from memory
                _contentService.RemoveContentByMusicProvider(existingProvider.Identifier);
                MusicProviders.Remove(existingProvider);
            }

            // Where we will store the script once loaded from the file
            string script;

            // Check to see if the user has provided a script
            if (string.IsNullOrEmpty(manifest.Script))
                throw new ManifestInvalidException(InvalidManifestReason.MissingScript);

            // Try load the source code file
            try
            {
                // Load the source code file
                var sourceCodeFile = Path.Combine(path, manifest.Script);
                script = File.ReadAllText(sourceCodeFile);
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex);
                // The script is missing or unreadable
                throw new ManifestInvalidException(InvalidManifestReason.MissingScript, ex);
            }

            // If the music provider is not packed, we need to pack it (copy the music provider into the music providers folder)
            if (!packed)
            {
                var newPath = Path.Combine(MusicProviderPath, manifest.Id.ToString());
                Directory.Move(path, newPath);
                path = newPath;
            }

            // Create the music provider and validate the rest of the manifest file
            var musicProvider = new MusicProvider(manifest, script, path, _settingsService, _mvxNavigationService, _authenticationService, _logProvider);
            MusicProviders.Add(musicProvider);
        }
    }
}