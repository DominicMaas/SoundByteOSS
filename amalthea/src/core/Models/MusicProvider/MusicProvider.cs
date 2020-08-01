using MvvmCross;
using MvvmCross.Logging;
using MvvmCross.Navigation;
using MvvmCross.ViewModels;
using SoundByte.Core.Engine;
using SoundByte.Core.Engine.Platform;
using SoundByte.Core.Exceptions;
using SoundByte.Core.Models.Content;
using SoundByte.Core.Models.Content.Buttons;
using SoundByte.Core.Services.Definitions;
using SoundByte.Core.Sources;
using System;
using System.Collections.Generic;
using Xamarin.Essentials;

namespace SoundByte.Core.Models.MusicProvider
{
    /// <summary>
    ///     Represents an application music provider
    /// </summary>
    public class MusicProvider : MvxNotifyPropertyChanged
    {
        /// <summary>
        ///     The unique identifier for this music provider
        /// </summary>
        public Guid Identifier { get; }

        /// <summary>
        ///     Manifest file for the music provider.
        /// </summary>
        public Manifest Manifest { get; }

        #region Updating

        /// <summary>
        ///     If the music provider is currently updating
        /// </summary>
        public bool IsUpdating
        {
            get => _isUpdating;
            set => SetProperty(ref _isUpdating, value);
        }

        private bool _isUpdating;

        /// <summary>
        ///     Is updating, what is the current percentage of this update
        /// </summary>
        public int UpdatePercentage
        {
            get => _updatePercentage;
            set => SetProperty(ref _updatePercentage, value);
        }

        private int _updatePercentage;

        #endregion Updating

        // JS engine pool for this music provider
        private readonly EnginePool _enginePool;

        public static void EnsureManifestValid(Manifest manifest)
        {
            // If the id is missing
            if (manifest.Id == null)
                throw new ManifestInvalidException(InvalidManifestReason.MissingId);

            // If the name is missing
            if (string.IsNullOrEmpty(manifest.Name))
                throw new ManifestInvalidException(InvalidManifestReason.MissingName);

            // If the publisher is missing
            if (string.IsNullOrEmpty(manifest.Publisher))
                throw new ManifestInvalidException(InvalidManifestReason.MissingPublisher);

            // Validate the provided music provider version
            if (string.IsNullOrEmpty(manifest.Version) || !Version.TryParse(manifest.Version, out _))
                throw new ManifestInvalidException(InvalidManifestReason.MissingVersion);

            // If there is no platform version specified
            if (!manifest.PlatformVersion.HasValue)
                throw new ManifestInvalidException(InvalidManifestReason.MissingPlatformVersion);

            // Check to see if we support this version
            if (manifest.PlatformVersion.Value > 1.0) // TODO, take this out
                throw new ManifestInvalidException(InvalidManifestReason.UnsupportedPlatformVersion);

            // If the description is missing
            if (string.IsNullOrEmpty(manifest.Description))
                throw new ManifestInvalidException(InvalidManifestReason.MissingDescription);

            // If the short description is missing, set it to the main description
            if (string.IsNullOrEmpty(manifest.ShortDescription))
                manifest.ShortDescription = manifest.Description;

            // If there is no url, set it to the store URL
            if (string.IsNullOrEmpty(manifest.Url))
                manifest.Url = "https://soundbytemedia.com/store/" + manifest.Id.ToString();

            // Check that assets are included
            if (manifest.Assets == null || string.IsNullOrEmpty(manifest.Assets.StoreLogo))
                throw new ManifestInvalidException(InvalidManifestReason.MissingAssets);

            // Check to see if the user has provided a script
            if (string.IsNullOrEmpty(manifest.Script))
                throw new ManifestInvalidException(InvalidManifestReason.MissingScript);
        }

        public MusicProvider(Manifest manifest, string javaScript, string path, ISettingsService settingsService, IMvxNavigationService mvxNavigationService, IAuthenticationService authenticationService, IMvxLogProvider logProvider)
        {
            EnsureManifestValid(manifest);

            // If the platform is specified, make sure ios is included // TODO, this needs to work cross platform
            if (manifest.Platforms != null)
            {
                if (!manifest.Platforms.Contains(DeviceInfo.Platform.ToString().ToLower()))
                    throw new ManifestInvalidException(InvalidManifestReason.PlatformUnsupported);
            }

            // Fix the logos
            manifest.Assets.StoreLogo = path + "/" + manifest.Assets.StoreLogo;

            Identifier = manifest.Id.Value;
            Manifest = manifest;

            // Setup the music provider
            SetupMusicProvider(manifest, authenticationService);

            // Create the JavaScript engine pool
            _enginePool = new EnginePool(new EnginePool.EnginePoolConfig(engine =>
            {
                // Bind the platform
                engine.SetValue("soundbyte", new Platform(engine, this, settingsService, mvxNavigationService, authenticationService, logProvider));

                // Load the music provider
                engine.Execute(javaScript);
            }));
        }

        private void SetupMusicProvider(Manifest provider, IAuthenticationService authenticationService)
        {
            // Setup the content groups
            if (provider.Content != null)
            {
                foreach (var group in provider.Content)
                {
                    // Extract any passed in buttons
                    var buttons = new List<ContentButton>();
                    if (group.Buttons != null)
                    {
                        foreach (var buttonString in group.Buttons)
                        {
                            switch (buttonString)
                            {
                                case "PlayAllButton":
                                    buttons.Add(new PlayContentButton());
                                    break;

                                case "ShufflePlayButton":
                                    buttons.Add(new ShufflePlayContentButton());
                                    break;
                            }
                        }
                    }

                    // Create the source and group
                    var source = new MusicProviderSource(this, group.IsAuthenticatedFeed, group.OnGet, authenticationService);

                    if (group.OnViewMore == null)
                    {
                        var contentGroup = new ContentGroup(source, group.Title, group.IsAuthenticatedFeed, buttons);
                        var location = (ContentArea)Enum.Parse(typeof(ContentArea), group.Area);
                        Mvx.IoCProvider.Resolve<IContentService>().AddContent(Identifier, location, contentGroup);
                    }
                    else
                    {
                        var contentGroup = new ContentGroup(source, group.Title, group.IsAuthenticatedFeed, buttons, parent => CallFunction(group.OnViewMore, parent));
                        var location = (ContentArea)Enum.Parse(typeof(ContentArea), group.Area);
                        Mvx.IoCProvider.Resolve<IContentService>().AddContent(Identifier, location, contentGroup);
                    }
                }
            }
        }

        public T CallFunction<T>(string function, params object[] arguments) where T : class
        {
            using var engine = _enginePool.GetEngine();
            return engine.CallFunction<T>(function, arguments);
        }

        public void CallFunction(string function, params object[] arguments)
        {
            using var engine = _enginePool.GetEngine();
            engine.CallFunction(function, arguments);
        }
    }
}