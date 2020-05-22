using JetBrains.Annotations;
using SoundByte.Core.Exceptions;
using SoundByte.Core.Items.Generic;
using SoundByte.Core.Items.Podcast;
using SoundByte.Core.Items.Track;
using SoundByte.Core.Items.User;
using SoundByte.Core.Services;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace SoundByte.Core.Sources.Podcast
{
    
    public class PodcastItemSource : ISource
    {
        /// <summary>
        ///     Podcast show source
        /// </summary>
        public BasePodcast Show { get; set; }

        public override Dictionary<string, object> GetParameters()
        {
            return new Dictionary<string, object>
            {
                { "s", Show.FeedUrl }
            };
        }

        public override void ApplyParameters(Dictionary<string, object> data)
        {
            data.TryGetValue("s", out var show);
            Show = new BasePodcast
            {
                FeedUrl = show.ToString()
            };
        }

        public override async Task<SourceResponse> GetItemsAsync(int count, string token, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (Show == null)
                throw new SoundByteException("Not Loaded", "Items not loaded yet.");

            var tracks = new List<BaseSoundByteItem>();

            try
            {
                using (var request = await HttpService.Instance.Client.GetAsync(Show.FeedUrl, cancellationToken).ConfigureAwait(false))
                {
                    request.EnsureSuccessStatusCode();

                    using (var stream = await request.Content.ReadAsStreamAsync())
                    {
                        // Load the document
                        var xmlDocument = XDocument.Load(stream);

                        // Get channel
                        var channel = xmlDocument.Root?.Element("channel");

                        // Get all the feed items
                        var feedItems = channel?.Elements("item");

                        XNamespace ns = "http://www.itunes.com/dtds/podcast-1.0.dtd";

                        foreach (var feedItem in feedItems)
                        {
                            tracks.Add(new BaseSoundByteItem(new BaseTrack
                            {
                                TrackId = feedItem.Element("guid")?.Value,
                                ServiceType = ServiceTypes.ITunesPodcast,
                                Title = feedItem.Element("title")?.Value,
                                //     Created = DateTime.Parse(feedItem.Element("pubDate")?.Value), //todo later
                                ArtworkUrl = feedItem.Element(ns + "image")?.Value,
                                AudioStreamUrl = feedItem.Element("enclosure").Attribute("url")?.Value,
                                //    Duration = TimeSpan.Parse(feedItem.Element(ns + "duration")?.Value), //todo later
                                User = new BaseUser { Username = channel?.Element(ns + "author")?.Value },
                                Description = feedItem.Element("description")?.Value
                            }));
                        }
                    }
                }

                return new SourceResponse(tracks, "eol");
            }
            catch (HttpRequestException hex)
            {
                throw new SoundByteException("Error", hex.Message + "\n" + Show.FeedUrl);
            }
        }
    }
}