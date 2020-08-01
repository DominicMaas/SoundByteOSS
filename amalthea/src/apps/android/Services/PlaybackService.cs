using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SoundByte.Core.Models.Media;
using SoundByte.Core.Models.Playback;
using SoundByte.Core.Services.Definitions;
using SoundByte.Core.Sources;

namespace SoundByte.App.Android.Services
{
    public class PlaybackService : IPlaybackService
    {
        public event PlaybackServiceEventHandlers.MediaChangedEventHandler OnMediaChange;

        public event PlaybackServiceEventHandlers.StateChangedEventHandler OnStateChange;

        public bool IsQueueShuffled()
        {
            throw new NotImplementedException();
        }

        public Task ShuffleQueueAsync(bool shuffle)
        {
            throw new NotImplementedException();
        }

        public bool IsMediaMuted()
        {
            throw new NotImplementedException();
        }

        public void MuteMedia(bool mute)
        {
            throw new NotImplementedException();
        }

        public double GetMediaVolume()
        {
            throw new NotImplementedException();
        }

        public void SetMediaVolume(double volume)
        {
            throw new NotImplementedException();
        }

        public TimeSpan GetMediaPosition()
        {
            throw new NotImplementedException();
        }

        public void SetMediaPosition(TimeSpan value)
        {
            throw new NotImplementedException();
        }

        public double GetMediaPlaybackRate()
        {
            throw new NotImplementedException();
        }

        public void SetMediaPlaybackRate(double value)
        {
            throw new NotImplementedException();
        }

        public bool IsMediaRepeating()
        {
            throw new NotImplementedException();
        }

        public void RepeatMedia(bool repeat)
        {
            throw new NotImplementedException();
        }

        public void NextMedia()
        {
            throw new NotImplementedException();
        }

        public void PreviousMedia()
        {
            throw new NotImplementedException();
        }

        public void PauseMedia()
        {
            throw new NotImplementedException();
        }

        public void PlayMedia()
        {
            throw new NotImplementedException();
        }

        public void MoveToMedia(Media media)
        {
            throw new NotImplementedException();
        }

        public TimeSpan GetMediaDuration()
        {
            throw new NotImplementedException();
        }

        public PlaybackState GetPlaybackState()
        {
            throw new NotImplementedException();
        }

        public Media GetCurrentMedia()
        {
            throw new NotImplementedException();
        }

        public List<Track> GetQueue()
        {
            throw new NotImplementedException();
        }

        public string GetToken()
        {
            throw new NotImplementedException();
        }

        public ISource GetSource()
        {
            throw new NotImplementedException();
        }

        public Task StartMediaAsync(Media mediaToPlay, TimeSpan? startTime = null)
        {
            throw new NotImplementedException();
        }

        public Task StartRandomMediaAsync()
        {
            throw new NotImplementedException();
        }

        public Task<PlaybackInitializeResponse> InitializeAsync(ISource model, IEnumerable<Media> queue = null, string token = null)
        {
            throw new NotImplementedException();
        }
    }
}