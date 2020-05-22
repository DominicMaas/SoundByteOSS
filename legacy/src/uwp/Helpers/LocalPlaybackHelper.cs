using GalaSoft.MvvmLight.Ioc;
using SoundByte.Core;
using SoundByte.Core.Items.Generic;
using SoundByte.Core.Items.Track;
using SoundByte.Core.Items.User;
using SoundByte.Core.Sources.Generic;
using SoundByte.App.Uwp.Services;
using SoundByte.App.Uwp.ServicesV2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.AccessCache;

namespace SoundByte.App.Uwp.Helpers
{
    public static class LocalPlaybackHelper
    {
        private static List<BaseSoundByteItem> _tracks = new List<BaseSoundByteItem>();

        private static readonly List<string> SupportedMediaTypes = new List<string>
        {
            ".mp3",
            ".wav",
            ".ogg",
            ".flac",
            ".m4a",
            ".aif",
            ".wma",
            ".mp4"
        };

        public static async Task StartLocalPlaybackAsync(IEnumerable<IStorageItem> files)
        {
            // Clear the list
            _tracks.Clear();

            await AddMusicFilesAsync(files);

            var result = await SimpleIoc.Default.GetInstance<IPlaybackService>().InitializePlaylistAsync(new DummyTrackSource(), _tracks);
            if (result.Success)
            {
                await SimpleIoc.Default.GetInstance<IPlaybackService>().StartTrackAsync(_tracks[0].Track);
            }
            else
            {
                await NavigationService.Current.CallMessageDialogAsync(result.Message);
            }
        }

        private static string GetNullString(string text, string backupText)
        {
            return string.IsNullOrEmpty(text)
                ? backupText
                : text;
        }

        private static async Task AddMusicFilesAsync(IEnumerable<IStorageItem> filesInput)
        {
            if (filesInput == null)
                return;

            // Get the files list
            var files = filesInput.ToList();

            // If we only opened one file, include other files in the folder
            if (files.Count == 1)
            {
                var musicFile = files[0] as StorageFile;
                var parentFolder = await musicFile.GetParentAsync();

                foreach (var parentFile in await parentFolder.GetFilesAsync())
                {
                    files.Add(parentFile);
                }
            }

            foreach (var item in files)
            {
                if (!(item is StorageFile file))
                    continue;

                if (!SupportedMediaTypes.Contains(file.FileType))
                    continue;

                var properties = await file.Properties.GetMusicPropertiesAsync();

                var track = new BaseTrack
                {
                    ServiceType = ServiceTypes.Local,
                    AudioStreamUrl = file.Path,
                    Title = GetNullString(properties.Title, file.DisplayName),
                    ArtworkUrl = "https://static.saavncdn.com/_i/3.0/album-default.png",
                    ThumbnailUrl = "https://static.saavncdn.com/_i/3.0/album-default.png",
                    User = new BaseUser
                    {
                        ServiceType = ServiceTypes.Local,
                        Username = GetNullString(properties.Artist, "Unknown Artist"),
                        ArtworkUrl = "http://a1.sndcdn.com/images/default_avatar_large.png",
                        ThumbnailUrl = "http://a1.sndcdn.com/images/default_avatar_large.png"
                    },
                    Genre = string.Join(',', properties.Genre),
                    Duration = properties.Duration,
                    TrackId = TextHelper.CalculateMd5Hash(file.Name + DateTime.UtcNow.ToString("O"))
                };

                // Store file for future access
                track.CustomProperties["file_token"] = StorageApplicationPermissions.FutureAccessList.Add(file);

                // Save the extension so we know if we should allow video playback
                track.CustomProperties["extension"] = file.FileType;

                _tracks.Add(new BaseSoundByteItem(track));
            }
        }
    }
}