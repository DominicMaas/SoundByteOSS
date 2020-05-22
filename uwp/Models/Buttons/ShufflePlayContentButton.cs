using SoundByte.App.Uwp.Commands;
using SoundByte.App.Uwp.ViewModels;

namespace SoundByte.App.Uwp.Models.Buttons
{
    /// <summary>
    ///     Button that shuffle plays all content. Only track items are supported.
    /// </summary>
    public class ShufflePlayContentButton : ContentButton
    {
        /// <summary>
        ///     Setup default values for the shuffle play button
        /// </summary>
        public ShufflePlayContentButton() : base("Shuffle Play", "\uE8B1", false, new DelegateCommand<ContentGroup>(OnClick))
        { }

        /// <summary>
        ///     Private click event to shuffle play all songs
        /// </summary>
        /// <param name="parent">Parent, used to get the list</param>
        private static async void OnClick(ContentGroup parent)
        {
            // Play all the tracks
            await BaseViewModel.ShufflePlayAllTracksAsync(parent.Collection);
        }
    }
}