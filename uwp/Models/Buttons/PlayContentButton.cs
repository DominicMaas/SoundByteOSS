using SoundByte.App.Uwp.Commands;
using SoundByte.App.Uwp.Common;
using SoundByte.App.Uwp.ViewModels;
using System.Threading.Tasks;

namespace SoundByte.App.Uwp.Models.Buttons
{
    /// <summary>
    ///     Button that plays all content. Only track items are supported.
    /// </summary>
    public class PlayContentButton : ContentButton
    {
        /// <summary>
        ///     Setup default values for the play all button
        /// </summary>
        public PlayContentButton() : base("Play All", "\uE768", false, new DelegateCommand<ContentGroup>(OnClick))
        { }

        /// <summary>
        ///     Private click event to play all songs
        /// </summary>
        /// <param name="parent">Parent, used to get the list</param>
        private static void OnClick(ContentGroup parent) => OnClickAsync(parent).FireAndForgetSafeAsync();

        private static async Task OnClickAsync(ContentGroup parent)
        {
            // Play all the tracks
            await BaseViewModel.PlayAllTracksAsync(parent.Collection);
        }
    }
}