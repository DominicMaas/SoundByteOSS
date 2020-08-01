using MvvmCross;
using MvvmCross.Commands;
using SoundByte.Core.Services.Definitions;

namespace SoundByte.Core.Models.Content.Buttons
{
    /// <summary>
    ///     Button that shuffle plays all content. Only track items are supported.
    /// </summary>
    public class ShufflePlayContentButton : ContentButton
    {
        public ShufflePlayContentButton() : base("Shuffle Play", "\uE8B1", false, new MvxAsyncCommand<ContentGroup>(async c =>
        {
            var playbackService = Mvx.IoCProvider.Resolve<IPlaybackService>();
            var dialogService = Mvx.IoCProvider.Resolve<IDialogService>();

            // Try build the playlist
            var result = await playbackService.InitializeAsync(c.Collection.Source, c.Collection, c.Collection.Token);
            if (!result.Success)
            {
                await dialogService.ShowErrorMessageAsync("Could not build playlist", result.Message);
                return;
            }

            // Start playback
            await playbackService.StartRandomMediaAsync();
        }))
        { }
    }
}