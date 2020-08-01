using System.Linq;
using System.Threading.Tasks;
using MvvmCross;
using MvvmCross.Navigation;
using SoundByte.Core.Models.Core;
using SoundByte.Core.Models.Media;
using SoundByte.Core.Services.Definitions;
using SoundByte.Core.Sources;
using SoundByte.Core.ViewModels.Details;

namespace SoundByte.Core.Helpers
{
    public static class ItemInvokeHelper
    {
        public static async Task InvokeItem<T>(SoundByteCollection<T> collection, Media item) where T : ISource
        {
            var provider = Mvx.IoCProvider.Resolve<IMusicProviderService>().MusicProviders.FirstOrDefault(x => x.Identifier == item.MusicProviderId);
            if (provider == null)
            {
                await Mvx.IoCProvider.Resolve<IDialogService>().ShowErrorMessageAsync("Cannot open item", $"You need the music provider ({item.MusicProviderId}) in order to interact with this item");
                return;
            }

            // Loop through all the item types
            switch (item.MediaType)
            {
                case MediaType.Unknown:
                    break;

                case MediaType.Track:
                case MediaType.PodcastEpisode:
                    // Build the queue
                    var result = await Mvx.IoCProvider.Resolve<IPlaybackService>().InitializeAsync(collection.Source, collection, collection.Token);
                    if (!result.Success)
                    {
                        await Mvx.IoCProvider.Resolve<IDialogService>().ShowErrorMessageAsync("Could not build playback queue", result.Message);
                        return;
                    }

                    // Play the song
                    await Mvx.IoCProvider.Resolve<IPlaybackService>().StartMediaAsync(item);
                    break;

                // Navigate to the user detail view
                case MediaType.User:
                    await Mvx.IoCProvider.Resolve<IMvxNavigationService>().Navigate<UserDetailViewModel, User>((User)item);
                    break;

                // Navigate to the playlist detail view
                case MediaType.Playlist:
                    await Mvx.IoCProvider.Resolve<IMvxNavigationService>().Navigate<PlaylistDetailViewModel, Playlist>((Playlist)item);
                    break;

                // Navigate to the podcast detail view
                case MediaType.PodcastShow:
                    await Mvx.IoCProvider.Resolve<IMvxNavigationService>().Navigate<PodcastDetailViewModel, PodcastShow>((PodcastShow)item);
                    break;
            }
        }
    }
}
