using GalaSoft.MvvmLight.Ioc;
using SoundByte.Core.Items.Track;
using SoundByte.Core.Sources;
using SoundByte.App.Uwp.Helpers;
using SoundByte.App.Uwp.Services;
using SoundByte.App.Uwp.ServicesV2;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace SoundByte.App.Uwp.ViewModels
{
    /// <summary>
    ///     Base class for all view models to extend off of
    /// </summary>
    public class BaseViewModel : INotifyPropertyChanged, IDisposable
    {
        /// <summary>
        ///     Dispose the model
        /// </summary>
        public virtual void Dispose()
        { }

        /// <summary>
        ///     Helper method for PlayAllTracksAsync to play shuffled tracks. See
        ///     PlayAllTracksAsync for more information
        /// </summary>
        /// <typeparam name="TSource">A base track source.</typeparam>
        /// <param name="model">SoundByte collection of a base track source.</param>
        public static async Task ShufflePlayAllTracksAsync<TSource>(SoundByteCollection<TSource> model) where TSource : ISource
        {
            await PlayAllTracksAsync(model, null, true);
        }

        /// <summary>
        ///     Attempts to play a model of type BaseTrack. This method handles playback and loading
        ///     feedback to the user. If an error occured, the user will see a message dialog.
        /// </summary>
        /// <typeparam name="TSource">A base track source.</typeparam>
        /// <param name="model">SoundByte collection of a base track source.</param>
        /// <param name="startingTrack">The track to start first (can be null)</param>
        /// <param name="shuffle">Should the playlist be shuffled.</param>
        public static async Task PlayAllTracksAsync<TSource>(SoundByteCollection<TSource> model, BaseTrack startingTrack = null, bool shuffle = false) where TSource : ISource
        {
            if (model.Count == 0)
                return;

            // Attempt to create the playlist
            var result = await SimpleIoc.Default.GetInstance<IPlaybackService>().InitializePlaylistAsync(model.Source, model, model.Token);

            if (result.Success == false)
            {
                await NavigationService.Current.CallMessageDialogAsync(result.Message, "Could not build playlist.");

                return;
            }

            // Start playback
            if (shuffle)
            {
                await SimpleIoc.Default.GetInstance<IPlaybackService>().StartRandomTrackAsync();
            }
            else
            {
                await SimpleIoc.Default.GetInstance<IPlaybackService>().StartTrackAsync(startingTrack);
            }
        }

        #region Property Changed Event Handlers

        public event PropertyChangedEventHandler PropertyChanged;

        // This method is called by the Set accessor of each property.
        // The CallerMemberName attribute that is applied to the optional propertyName
        // parameter causes the property name of the caller to be substituted as an argument.
        protected void UpdateProperty([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion Property Changed Event Handlers
    }
}