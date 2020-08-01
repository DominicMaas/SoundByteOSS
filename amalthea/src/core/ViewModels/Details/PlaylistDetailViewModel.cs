using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MvvmCross;
using MvvmCross.Commands;
using MvvmCross.ViewModels;
using SoundByte.Core.Helpers;
using SoundByte.Core.Models.Core;
using SoundByte.Core.Models.Media;
using SoundByte.Core.Services.Definitions;
using SoundByte.Core.Sources;

namespace SoundByte.Core.ViewModels.Details
{
    /// <summary>
    ///     View more information about a playlist / view items within the playlist
    /// </summary>
    public class PlaylistDetailViewModel : MvxViewModel<Playlist>
    {
        #region Commands

        public IMvxAsyncCommand PlayCommand { get; }
        public IMvxAsyncCommand<Media> PlayItemCommand { get; }
        public IMvxAsyncCommand ShufflePlayCommand { get; }
        public IMvxAsyncCommand RefreshCommand { get; }

        #endregion Commands

        /// <summary>
        ///     The playlist that is currently being displayed on the screen
        /// </summary>
        public Playlist Playlist
        {
            get => _playlist;
            set => SetProperty(ref _playlist, value);
        }

        private Playlist _playlist;

        public SoundByteCollection<MusicProviderSource> Tracks { get; private set; }

        private readonly IMusicProviderService _musicProviderService;
        private readonly IAuthenticationService _authenticationService;

        public PlaylistDetailViewModel(IMusicProviderService musicProviderService, IAuthenticationService authenticationService)
        {
            _musicProviderService = musicProviderService;
            _authenticationService = authenticationService;

            // Bind commands
            PlayCommand = new MvxAsyncCommand(async () => await PlayAsync(false));
            PlayItemCommand = new MvxAsyncCommand<Media>(async item => await ItemInvokeHelper.InvokeItem(Tracks, item));
            ShufflePlayCommand = new MvxAsyncCommand(async () => await PlayAsync(true));
            RefreshCommand = new MvxAsyncCommand(async () => await Tracks.RefreshItemsAsync());
        }

        /// <summary>
        ///     Prepare the view model for display
        /// </summary>
        /// <param name="parameter">The parameter that the user wants to view</param>
        public override void Prepare(Playlist parameter)
        {
            Playlist = parameter;

            var provider = _musicProviderService.MusicProviders.FirstOrDefault(x => x.Identifier == parameter.MusicProviderId);
            var resolver = provider?.Manifest?.Resolvers?.PlaylistTracks;

            var source = new MusicProviderSource(provider, false, resolver, _authenticationService);
            source.ApplyParameters(new Dictionary<string, string>
            {
                { "playlistId", parameter.PlaylistId }
            });

            // MusicProviderSource supports null providers and resolvers (it handles them)
            Tracks = new SoundByteCollection<MusicProviderSource>(source);
        }

        public override async Task Initialize()
        {
            await Tracks.RefreshItemsAsync();
        }

        private async Task PlayAsync(bool shuffle)
        {
            // Build the queue
            var result = await Mvx.IoCProvider.Resolve<IPlaybackService>().InitializeAsync(Tracks.Source, Tracks, Tracks.Token);
            if (!result.Success)
            {
                await Mvx.IoCProvider.Resolve<IDialogService>().ShowErrorMessageAsync("Could not build playback queue", result.Message);
                return;
            }

            // Play the song
            if (shuffle)
            {
                await Mvx.IoCProvider.Resolve<IPlaybackService>().StartRandomMediaAsync();
            }
            else
            {
                await Mvx.IoCProvider.Resolve<IPlaybackService>().StartMediaAsync(Tracks.FirstOrDefault());
            }
        }
    }
}