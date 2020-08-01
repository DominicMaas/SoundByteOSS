using MvvmCross.Commands;

namespace SoundByte.Core.Models.Content.Buttons
{
    /// <summary>
    ///     Button for creating playlists
    /// </summary>
    public class CreatePlaylistContentButton : ContentButton
    {
        public CreatePlaylistContentButton() : base("Create Playlist", "\uE710", false, new MvxAsyncCommand<ContentGroup>(async c =>
        {
        }))
        { }
    }
}