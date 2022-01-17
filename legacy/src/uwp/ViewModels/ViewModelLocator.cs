using CommonServiceLocator;
using GalaSoft.MvvmLight.Ioc;
using SoundByte.App.Uwp.Dialogs;
using SoundByte.App.Uwp.Services;
using SoundByte.App.Uwp.ServicesV2;
using SoundByte.App.Uwp.ServicesV2.Implementations;
using SoundByte.App.Uwp.ViewModels.Navigation;
using SoundByte.App.Uwp.ViewModels.Panes;
using SoundByte.App.Uwp.ViewModels.Playback;
using SoundByte.App.Uwp.ViewModels.Xbox;

namespace SoundByte.App.Uwp.ViewModels
{
    public class ViewModelLocator
    {
        public static void EarlyInit()
        {
            // These must be init asap
            ServiceLocator.SetLocatorProvider(() => SimpleIoc.Default);
            SimpleIoc.Default.Register<ITelemetryService, TelemetryService>(true);
            SimpleIoc.Default.Register<IContentService, ContentService>(true);
            SimpleIoc.Default.Register<IExtensionService, ExtensionService>(true);
            SimpleIoc.Default.Register<IPlaybackService, PlaybackService>(true);
        }

        public ViewModelLocator()
        {
            // Register the dialogs
            NavigationService.Current.RegisterTypeAsDialog<PinTileDialog>();
            NavigationService.Current.RegisterTypeAsDialog<AddToPlaylistDialog>();
            NavigationService.Current.RegisterTypeAsDialog<ShareDialog>();
            NavigationService.Current.RegisterTypeAsDialog<CreatePlaylistDialog>();
            NavigationService.Current.RegisterTypeAsDialog<ContinueOnDeviceDialog>();
            NavigationService.Current.RegisterTypeAsDialog<CastTrackDialog>();
            NavigationService.Current.RegisterTypeAsDialog<GoogleCastTrackDialog>();
            NavigationService.Current.RegisterTypeAsDialog<AirPlayCastTrackDialog>();
            NavigationService.Current.RegisterTypeAsDialog<ManageExtensionAccountsDialog>();
            NavigationService.Current.RegisterTypeAsDialog<ManageMusicProvidersDialog>();
            NavigationService.Current.RegisterTypeAsDialog<AboutDialog>();

            // Register sources
            App.SourceManager.RegisterDefaultSources();
        }

        #region Panes

        public ExtensionPaneViewModel ExtensionPane
        {
            get
            {
                if (!SimpleIoc.Default.ContainsCreated<ExtensionPaneViewModel>())
                    SimpleIoc.Default.Register<ExtensionPaneViewModel>();

                return SimpleIoc.Default.GetInstance<ExtensionPaneViewModel>();
            }
        }

        #endregion Panes

        /// <summary>
        ///     Get the home view model
        /// </summary>
        public HomeViewModel Home
        {
            get
            {
                if (!SimpleIoc.Default.ContainsCreated<HomeViewModel>())
                    SimpleIoc.Default.Register<HomeViewModel>();

                return SimpleIoc.Default.GetInstance<HomeViewModel>();
            }
        }

        public XboxMusicViewModel XboxMusic
        {
            get
            {
                if (!SimpleIoc.Default.ContainsCreated<XboxMusicViewModel>())
                    SimpleIoc.Default.Register<XboxMusicViewModel>();

                return SimpleIoc.Default.GetInstance<XboxMusicViewModel>();
            }
        }

        /// <summary>
        ///     Get the my music view model
        /// </summary>
        public MyMusicViewModel MyMusic
        {
            get
            {
                if (!SimpleIoc.Default.ContainsCreated<MyMusicViewModel>())
                    SimpleIoc.Default.Register<MyMusicViewModel>();

                return SimpleIoc.Default.GetInstance<MyMusicViewModel>();
            }
        }

        /// <summary>
        ///     Get the browse view model
        /// </summary>
        public BrowseViewModel Browse
        {
            get
            {
                if (!SimpleIoc.Default.ContainsCreated<BrowseViewModel>())
                    SimpleIoc.Default.Register<BrowseViewModel>();

                return SimpleIoc.Default.GetInstance<BrowseViewModel>();
            }
        }

        /// <summary>
        ///     Get the search view model
        /// </summary>
        public SearchViewModel Search
        {
            get
            {
                if (!SimpleIoc.Default.ContainsCreated<SearchViewModel>())
                    SimpleIoc.Default.Register<SearchViewModel>();

                return SimpleIoc.Default.GetInstance<SearchViewModel>();
            }
        }

        /// <summary>
        ///     Get the playback
        /// </summary>
        public PlaybackViewModel Playback
        {
            get
            {
                if (!SimpleIoc.Default.ContainsCreated<PlaybackViewModel>())
                    SimpleIoc.Default.Register<PlaybackViewModel>();

                return SimpleIoc.Default.GetInstance<PlaybackViewModel>();
            }
        }

        public void CleanViewModels()
        {
            if (SimpleIoc.Default.IsRegistered<HomeViewModel>())
            {
                SimpleIoc.Default.GetInstance<HomeViewModel>().Dispose();
                SimpleIoc.Default.Unregister<HomeViewModel>();
            }

            if (SimpleIoc.Default.IsRegistered<MyMusicViewModel>())
            {
                SimpleIoc.Default.GetInstance<MyMusicViewModel>().Dispose();
                SimpleIoc.Default.Unregister<MyMusicViewModel>();
            }

            if (SimpleIoc.Default.IsRegistered<BrowseViewModel>())
            {
                SimpleIoc.Default.GetInstance<BrowseViewModel>().Dispose();
                SimpleIoc.Default.Unregister<BrowseViewModel>();
            }

            if (SimpleIoc.Default.IsRegistered<SearchViewModel>())
            {
                SimpleIoc.Default.GetInstance<SearchViewModel>().Dispose();
                SimpleIoc.Default.Unregister<SearchViewModel>();
            }

            if (SimpleIoc.Default.IsRegistered<PlaybackViewModel>())
            {
                SimpleIoc.Default.GetInstance<PlaybackViewModel>().Dispose();
                SimpleIoc.Default.Unregister<PlaybackViewModel>();
            }

            if (SimpleIoc.Default.IsRegistered<XboxMusicViewModel>())
            {
                SimpleIoc.Default.GetInstance<XboxMusicViewModel>().Dispose();
                SimpleIoc.Default.Unregister<XboxMusicViewModel>();
            }
        }
    }
}