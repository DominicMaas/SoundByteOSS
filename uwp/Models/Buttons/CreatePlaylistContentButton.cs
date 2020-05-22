using SoundByte.App.Uwp.Commands;
using SoundByte.App.Uwp.Dialogs;
using SoundByte.App.Uwp.Services;

namespace SoundByte.App.Uwp.Models.Buttons
{
    /// <summary>
    ///     Button for creating playlists
    /// </summary>
    public class CreatePlaylistContentButton : ContentButton
    {
        public CreatePlaylistContentButton(int serviceType) : base("Create Playlist", "\uE710", false, new DelegateCommand<ContentGroup>(
            async s =>
            {
                await NavigationService.Current.CallDialogAsync<CreatePlaylistDialog>(serviceType);
            }))
        { }
    }
}