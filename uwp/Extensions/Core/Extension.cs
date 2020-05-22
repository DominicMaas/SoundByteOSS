#nullable enable

using GalaSoft.MvvmLight.Ioc;
using SoundByte.App.Uwp.Extensions.Definitions;
using SoundByte.App.Uwp.Extensions.Exceptions;
using SoundByte.App.Uwp.Models;
using SoundByte.App.Uwp.Models.Buttons;
using SoundByte.App.Uwp.ServicesV2;
using SoundByte.App.Uwp.Sources;
using SoundByte.App.Uwp.ViewModels.Generic;
using SoundByte.Core.Items.Generic;
using SoundByte.Core.Items.Playlist;
using SoundByte.Core.Items.Track;
using SoundByte.Core.Items.User;
using SoundByte.Core.Models.Content;
using SoundByte.Core.Models.Navigation;
using SoundByte.Core.Sources;
using System;
using System.Collections.Generic;
using Windows.Storage;

namespace SoundByte.App.Uwp.Extensions.Core
{
    /// <summary>
    ///     Represents an application extension
    /// </summary>
    public class Extension
    {
        /// <summary>
        ///     The unique identifier for this extension
        /// </summary>
        public Guid Identifier { get; }

        /// <summary>
        ///     Manifest file for the extension.
        /// </summary>
        public ExtensionManifest Manifest { get; }

        /// <summary>
        ///     If the extension is packed, or loaded from an external source.
        /// </summary>
        public bool IsPacked { get; }

        /// <summary>
        ///     The folder where this extension is located.
        /// </summary>
        public StorageFolder Folder { get; }

        // JS engine pool for this extension
        private readonly EnginePool _enginePool;

        public Extension(StorageFolder folder, ExtensionManifest manifest, bool isPacked, string javaScript, string javaScriptLibrary)
        {
            // If the id is missing
            if (manifest.Id == null)
                throw new ManifestInvalidException(InvalidManifestError.MissingId);

            // If the name is missing
            if (string.IsNullOrEmpty(manifest.Name))
                throw new ManifestInvalidException(InvalidManifestError.MissingName);

            // If the publisher is missing
            if (string.IsNullOrEmpty(manifest.Publisher))
                throw new ManifestInvalidException(InvalidManifestError.MissingPublisher);

            // Validate the provided extension version
            if (string.IsNullOrEmpty(manifest.Version) || !Version.TryParse(manifest.Version, out _))
                throw new ManifestInvalidException(InvalidManifestError.MissingVersion);

            // If there is no platform version specified
            if (!manifest.PlatformVersion.HasValue)
                throw new ManifestInvalidException(InvalidManifestError.MissingPlatformVersion);

            // Check to see if we support this version
            if (manifest.PlatformVersion.Value > ExtensionBootstrapper.ApiVersion)
                throw new ManifestInvalidException(InvalidManifestError.UnsupportedPlatformVersion);

            // If the description is missing
            if (string.IsNullOrEmpty(manifest.Description))
                throw new ManifestInvalidException(InvalidManifestError.MissingDescription);

            // If the short description is missing, set it to the main description
            if (string.IsNullOrEmpty(manifest.ShortDescription))
                manifest.ShortDescription = manifest.Description;

            // If there is no url, set it to the store URL
            if (string.IsNullOrEmpty(manifest.Url))
                manifest.Url = "https://soundbytemedia.com/store/" + manifest.Id.ToString();

            // If the platform is specified, make sure UWP is included
            if (manifest.Platforms != null)
            {
                if (!manifest.Platforms.Contains("uwp"))
                    throw new ManifestInvalidException(InvalidManifestError.PlatformUnsupported);
            }

            // Check that assets are included
            if (manifest.Assets == null || string.IsNullOrEmpty(manifest.Assets.StoreLogo))
                throw new ManifestInvalidException(InvalidManifestError.MissingAssets);

            // Fix the logos
            manifest.Assets.StoreLogo = folder.Path + "/" + manifest.Assets.StoreLogo;

            Identifier = manifest.Id.Value;
            Folder = folder;
            Manifest = manifest;
            IsPacked = isPacked;

            // Setup the music provider
            if (manifest.Provider != null)
                SetupMusicProvider(manifest.Provider);

            // Create the JavaScript engine pool
            _enginePool = new EnginePool(new EnginePool.EnginePoolConfig(engine =>
            {
                try
                {
                    // Add the class types the app can access
                    engine.SetValue("Track", Jint.Runtime.Interop.TypeReference.CreateTypeReference(engine, typeof(BaseTrack)));
                    engine.SetValue("User", Jint.Runtime.Interop.TypeReference.CreateTypeReference(engine, typeof(BaseUser)));
                    engine.SetValue("Playlist", Jint.Runtime.Interop.TypeReference.CreateTypeReference(engine, typeof(BasePlaylist)));
                    engine.SetValue("GenericItem", Jint.Runtime.Interop.TypeReference.CreateTypeReference(engine, typeof(BaseSoundByteItem)));
                    engine.SetValue("SourceResponse", Jint.Runtime.Interop.TypeReference.CreateTypeReference(engine, typeof(SourceResponse)));

                    engine.SetValue("FilteredListViewHolder", Jint.Runtime.Interop.TypeReference.CreateTypeReference(engine, typeof(FilteredListViewModel.Holder)));
                    engine.SetValue("FilterViewItem", Jint.Runtime.Interop.TypeReference.CreateTypeReference(engine, typeof(FilteredListViewModel.Filter)));
                    engine.SetValue("PageName", Jint.Runtime.Interop.TypeReference.CreateTypeReference(engine, typeof(PageName)));

                    // Bind the platform
                    engine.SetValue("platform", new Platform());

                    // Load the Soundbyte library
                    engine.Execute(javaScriptLibrary);

                    // Load the music provider
                    engine.Execute(javaScript);
                }
                catch (Exception e)
                {
                    App.NotificationManager?.Show($"{Manifest?.Name ?? "Unknown"} (extension) is causing script errors: " + e.Message, 5000);
                }
            }));
        }

        private void SetupMusicProvider(MusicProvider provider)
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
                    var source = new ExtensionSource(this, group.OnGet);
                    var contentGroup = new ContentGroup(source, group.Title, buttons, (parent) =>
                    {
                        if (group.OnViewMore != null)
                            CallFunction(group.OnViewMore, parent);
                    });

                    // Parse the area
                    var location = Enum.Parse<ContentArea>(group.Area);

                    // Add the content group
                    SimpleIoc.Default.GetInstance<IContentService>().AddContent(Identifier, location, contentGroup);
                }
            }
        }

        /// <summary>
        ///     Call the main function of the extension, this starts everything
        /// </summary>
        public T? CallFunction<T>(string function, params object[] arguments) where T : class
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

#nullable restore