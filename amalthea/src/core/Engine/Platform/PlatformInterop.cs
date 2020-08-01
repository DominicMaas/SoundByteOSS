using System.Linq;
using Jint.Native;
using SoundByte.Core.Helpers;
using Xamarin.Essentials;
using YoutubeExplode;

namespace SoundByte.Core.Engine.Platform
{
    public class PlatformInterop
    {
        public YouTubeInterop Youtube => new YouTubeInterop();

        public class YouTubeInterop
        {
            private static readonly YoutubeClient _youTubeClient = new YoutubeClient();

            public JsValue GetVideoStream(string id)
            {
                return AsyncHelper.RunSync(async () =>
                {
                    var manifest = await _youTubeClient.Videos.Streams.GetManifestAsync(id);
                    return manifest.GetVideo().Where(x => !string.IsNullOrEmpty(x.Url)).OrderByDescending(q => q.Bitrate).First()?.Url;
                });
            }

            public JsValue GetAudioStream(string id)
            {
                if (DeviceInfo.Platform == DevicePlatform.iOS || DeviceInfo.Platform == DevicePlatform.tvOS ||
                    DeviceInfo.Platform == DevicePlatform.watchOS)
                {
                    // Apple devices only support AAC
                    return AsyncHelper.RunSync(async () =>
                    {
                        var manifest = await _youTubeClient.Videos.Streams.GetManifestAsync(id);
                        var stream = manifest.GetAudio().Where(x => !string.IsNullOrEmpty(x.Url) && x.AudioCodec.ToLower() == "aac").OrderBy(q => q.Bitrate).First();
                        return stream?.Url;
                    });
                }
                else
                {
                    return AsyncHelper.RunSync(async () =>
                    {
                        var manifest = await _youTubeClient.Videos.Streams.GetManifestAsync(id);
                        var stream = manifest.GetAudio().Where(x => !string.IsNullOrEmpty(x.Url)).OrderBy(q => q.Bitrate).First();
                        return stream?.Url;
                    });
                }
            }
        }
    }
}