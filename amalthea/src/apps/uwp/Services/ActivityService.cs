using Microsoft.Toolkit.Uwp.Helpers;
using SoundByte.Core.Models.Media;
using SoundByte.Core.Services.Definitions;
using SoundByte.Core.Sources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Windows.ApplicationModel.UserActivities;
using Windows.UI.Shell;

namespace SoundByte.App.UWP.Services
{
    public class ActivityService : IActivityService
    {
        private UserActivitySession _currentUserActivitySession;
        private UserActivityChannel _channel;

        public ActivityService()
        {
            _channel = UserActivityChannel.GetDefault();
        }

        public async Task UpdateActivityAsync(ISource source, Media media, IEnumerable<Media> queue, string token, TimeSpan? timeSpan, bool isShuffled)
        {
            if (media.MediaType != MediaType.Track) return;
            var track = (Track)media;

            var activity = await _channel.GetOrCreateUserActivityAsync("last-playback");
            activity.FallbackUri = new Uri("https://soundbytemedia.com/pages/remote-subsystem");

            var continueText = @"Continue listening to " + track.Title.Replace('"', ' ') + " and " + queue.Count() + " other songs.";

            activity.VisualElements.DisplayText = track.Title.Replace('"', ' ');
            activity.VisualElements.Description = continueText;

            var json = @"{""$schema"":""http://adaptivecards.io/schemas/adaptive-card.json"",""type"":""AdaptiveCard"",""backgroundImage"":""" + track.ArtworkUrl + @""",""version"": ""1.0"",""body"":[{""type"":""Container"",""items"":[{""type"":""TextBlock"",""text"":""" + track.Title.Replace('"', ' ') + @""",""weight"":""bolder"",""size"":""large"",""wrap"":true,""maxLines"":3},{""type"":""TextBlock"",""text"":""" + continueText + @""",""size"":""default"",""wrap"":true,""maxLines"":3}]}]}";
            activity.VisualElements.Content = AdaptiveCardBuilder.CreateAdaptiveCardFromJson(json);

            activity.ActivationUri = new Uri("https://soundbytemedia.com/pages/remote-subsystem");

            await activity.SaveAsync();

            // Session
            await DispatcherHelper.ExecuteOnUIThreadAsync(() =>
            {
                _currentUserActivitySession?.Dispose();
                _currentUserActivitySession = null;

                _currentUserActivitySession = activity.CreateSession();
            });
        }
    }
}
