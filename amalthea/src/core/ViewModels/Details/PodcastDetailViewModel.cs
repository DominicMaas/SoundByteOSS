using MvvmCross.ViewModels;
using SoundByte.Core.Models.Media;

namespace SoundByte.Core.ViewModels.Details
{
    /// <summary>
    ///     View more details about a podcast and view episodes
    /// </summary>
    public class PodcastDetailViewModel : MvxViewModel<PodcastShow>
    {
        /// <summary>
        ///     The podcast that is currently being displayed on the screen
        /// </summary>
        public PodcastShow Podcast
        {
            get => _podcast;
            set => SetProperty(ref _podcast, value);
        }

        private PodcastShow _podcast;

        /// <summary>
        ///     Prepare the view model for display
        /// </summary>
        /// <param name="parameter">The parameter that the user wants to view</param>
        public override void Prepare(PodcastShow parameter)
        {
            Podcast = parameter;
        }
    }
}