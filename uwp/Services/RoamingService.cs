using GalaSoft.MvvmLight.Ioc;
using JetBrains.Annotations;
using Microsoft.Toolkit.Uwp.Helpers;
using SoundByte.Core;
using SoundByte.Core.Items.Generic;
using SoundByte.Core.Items.Track;
using SoundByte.Core.Sources;
using SoundByte.App.Uwp.Helpers;
using SoundByte.App.Uwp.ServicesV2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Windows.ApplicationModel.UserActivities;
using Windows.UI.Shell;

namespace SoundByte.App.Uwp.Services
{
    /// <summary>
    ///     Used for roaming content accross devices and platforms.
    /// </summary>
    public class RoamingService
    {
        private UserActivitySession _currentUserActivitySession;
        private UserActivityChannel _channel;

        public RoamingService()
        {
            _channel = UserActivityChannel.GetDefault();
        }

        public async Task<UserActivity> UpdateActivityAsync(ISource source, BaseTrack track, IEnumerable<BaseSoundByteItem> playlist, string token, TimeSpan? timeSpan, bool isShuffled)
        {
            // Don't enable if windows timeline support is disabled
            if (!SettingsService.Instance.WindowsTimelineEnabled)
                return null;

            // We do not support these items
            if (track.ServiceType == ServiceTypes.ITunesPodcast ||
                track.ServiceType == ServiceTypes.Local ||
                track.ServiceType == ServiceTypes.Unknown)
                return null;

            var activity = await _channel.GetOrCreateUserActivityAsync("playback-" + SettingsService.Instance.SessionId);
            activity.FallbackUri = new Uri("https://soundbytemedia.com/pages/remote-subsystem");

            var continueText = @"Continue listening to " + track.Title.Replace('"', ' ') + " and " + playlist.Count() + " other songs.";

            activity.VisualElements.DisplayText = track.Title.Replace('"', ' ');
            activity.VisualElements.Description = continueText;

            var json = @"{""$schema"":""http://adaptivecards.io/schemas/adaptive-card.json"",""type"":""AdaptiveCard"",""backgroundImage"":""" + track.ArtworkUrl + @""",""version"": ""1.0"",""body"":[{""type"":""Container"",""items"":[{""type"":""TextBlock"",""text"":""" + track.Title.Replace('"', ' ') + @""",""weight"":""bolder"",""size"":""large"",""wrap"":true,""maxLines"":3},{""type"":""TextBlock"",""text"":""" + continueText + @""",""size"":""default"",""wrap"":true,""maxLines"":3}]}]}";
            activity.VisualElements.Content = AdaptiveCardBuilder.CreateAdaptiveCardFromJson(json);

            // Set the activation url using shorthand protocol
            var protoUri = ProtocolHelper.EncodeTrackProtocolItem(new ProtocolHelper.TrackProtocolItem(source, new BaseSoundByteItem(track), playlist, token, timeSpan, isShuffled), true) + "&session=" + SettingsService.Instance.SessionId;
            activity.ActivationUri = new Uri(protoUri);

            await activity.SaveAsync();
            return activity;
        }

        public async Task StartActivityAsync(ISource source, BaseTrack track, IEnumerable<BaseSoundByteItem> playlist, string token, TimeSpan? timeSpan, bool isShuffled)
        {
            // Don't enable if windows timeline support is disabled
            if (!SettingsService.Instance.WindowsTimelineEnabled)
                return;

            try
            {
                // Don't run this logic if the track does not exist
                if (track == null)
                    return;

                // We do not support these items
                if (track.ServiceType == ServiceTypes.ITunesPodcast ||
                    track.ServiceType == ServiceTypes.Local ||
                    track.ServiceType == ServiceTypes.Unknown)
                    return;

                var activity = await UpdateActivityAsync(source, track, playlist, token, timeSpan, isShuffled);

                await DispatcherHelper.ExecuteOnUIThreadAsync(() =>
                {
                    _currentUserActivitySession = activity?.CreateSession();
                });
            }
            catch (Exception ex)
            {
                SimpleIoc.Default.GetInstance<ITelemetryService>().TrackException(ex);
            }
        }

        public async Task StopActivityAsync(TimeSpan? currentPosition = null)
        {
            // Don't enable if windows timeline support is disabled
            if (!SettingsService.Instance.WindowsTimelineEnabled)
                return;

            try
            {
                // If no track is playing, don't run this logic
                if (SimpleIoc.Default.GetInstance<IPlaybackService>().GetCurrentTrack() == null)
                    return;

                var track = SimpleIoc.Default.GetInstance<IPlaybackService>().GetCurrentTrack();
                var playlist = SimpleIoc.Default.GetInstance<IPlaybackService>().GetMediaPlaybackList().Items.Select(x => new BaseSoundByteItem(x.Source.AsBaseTrack()));
                var token = SimpleIoc.Default.GetInstance<IPlaybackService>().GetPlaylistToken();
                var source = SimpleIoc.Default.GetInstance<IPlaybackService>().GetPlaylistSource();

                await UpdateActivityAsync(source, track, playlist, token, currentPosition, SimpleIoc.Default.GetInstance<IPlaybackService>().IsPlaylistShuffled());

                _currentUserActivitySession?.Dispose();
            }
            catch (Exception ex)
            {
                SimpleIoc.Default.GetInstance<ITelemetryService>().TrackException(ex);
            }
        }
    }
}