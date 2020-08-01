using Jint.Runtime.Interop;
using MvvmCross.Navigation;
using SoundByte.Core.Models.Media;
using SoundByte.Core.Models.MusicProvider;
using SoundByte.Core.Models.Sources;
using SoundByte.Core.Services.Definitions;
using SoundByte.Core.ViewModels.Generic;
using System;
using MvvmCross.Logging;

namespace SoundByte.Core.Engine.Platform
{
    /// <summary>
    ///     Represents the music provider platform. All methods are provided through this class
    /// </summary>
    public class Platform
    {
        private readonly Jint.Engine _engine;
        private readonly MusicProvider _musicProvider;

        private readonly ISettingsService _settingsService;
        private readonly IMvxNavigationService _mvxNavigationService;
        private readonly IAuthenticationService _authenticationService;
        private readonly IMvxLogProvider _logProvider;

        public Platform(Jint.Engine engine, MusicProvider musicProvider, ISettingsService settingsService, IMvxNavigationService mvxNavigationService, IAuthenticationService authenticationService, IMvxLogProvider logProvider)
        {
            _engine = engine;
            _musicProvider = musicProvider;

            _settingsService = settingsService;
            _mvxNavigationService = mvxNavigationService;
            _authenticationService = authenticationService;
            _logProvider = logProvider;
        }

        public PlatformNavigation? Navigation
            => _musicProvider.Manifest.Permissions.Contains("navigation") ? new PlatformNavigation(_mvxNavigationService) : null;

        public PlatformSettings? Settings
            => _musicProvider.Manifest.Permissions.Contains("settings") ? new PlatformSettings(_settingsService, _musicProvider) : null;

        public PlatformNetwork? Network
           => _musicProvider.Manifest.Permissions.Contains("network") ? new PlatformNetwork(_logProvider, _authenticationService, _musicProvider) : null;

        public PlatformInterop? Interop
            => _musicProvider.Manifest.Permissions.Contains("interop") ? new PlatformInterop() : null;

        public TimeSpan TimeFromMilliseconds(double ms)
        {
            return TimeSpan.FromMilliseconds(ms);
        }

        public TypeReference MediaType => TypeReference.CreateTypeReference(_engine, typeof(MediaType));
        public TypeReference Media => TypeReference.CreateTypeReference(_engine, typeof(Media));
        public TypeReference Track => TypeReference.CreateTypeReference(_engine, typeof(Track));
        public TypeReference User => TypeReference.CreateTypeReference(_engine, typeof(User));
        public TypeReference Playlist => TypeReference.CreateTypeReference(_engine, typeof(Playlist));
        public TypeReference SourceResponse => TypeReference.CreateTypeReference(_engine, typeof(SourceResponse));
        public TypeReference FilteredListViewModelHolder => TypeReference.CreateTypeReference(_engine, typeof(FilteredListViewModel.Holder));
        public TypeReference FilterViewItem => TypeReference.CreateTypeReference(_engine, typeof(FilteredListViewModel.Filter));
        public TypeReference GenericListViewModelHolder => TypeReference.CreateTypeReference(_engine, typeof(GenericListViewModel.Holder));
    }
}