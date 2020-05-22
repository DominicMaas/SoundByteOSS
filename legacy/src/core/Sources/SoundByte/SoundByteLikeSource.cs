using JetBrains.Annotations;
using SoundByte.Core.Helpers;
using SoundByte.Core.Items.Generic;
using SoundByte.Core.Items.SoundByte;
using SoundByte.Core.Items.Track;
using SoundByte.Core.Services;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SoundByte.Core.Sources.SoundByte
{
    
    public class SoundByteLikeSource : ISource
    {
        public override Dictionary<string, object> GetParameters()
        {
            return new Dictionary<string, object>();
        }

        public override void ApplyParameters(Dictionary<string, object> data)
        {
            // Not used
        }

        public override async Task<SourceResponse> GetItemsAsync(int count, string token,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            // If the user has not connected their SoundByte account.
            if (!SoundByteService.Current.IsSoundByteAccountConnected)
            {
                return await Task.Run(() =>
                    new SourceResponse(null, null, false,
                        Resources.Resources.Sources_SoundByte_NoAccount_Title,
                        Resources.Resources.Sources_SoundByte_Like_NoAccount_Description));
            }

            var likes = await SoundByteService.Current.GetAsync<LikesOutputModel>(ServiceTypes.SoundByte, "likes", new Dictionary<string, string>
            {
                { "PageNumber", token },
                { "PageSize", "30" }
            }, cancellationToken).ConfigureAwait(false);

            if (!likes.Response.Items.Any())
            {
                return new SourceResponse(null, null, false, "No Likes", "Like a SoundCloud / Fanburst track or a YouTube music video to start!");
            }

            var nextPage = likes.Response.Links.FirstOrDefault(x => x.Rel == "next_page");

            if (nextPage == null)
                return new SourceResponse(likes.Response.Items.Select(x => new BaseSoundByteItem(x)), "eol");

            var param = new QueryParameterCollection(nextPage.Href);
            var nextToken = param.FirstOrDefault(x => x.Key == "PageNumber").Value;

            return new SourceResponse(likes.Response.Items.Select(x => new BaseSoundByteItem(x)), nextToken);
        }

        public class LikesOutputModel
        {
            public PagingHeader Paging { get; set; }
            public List<LinkInfo> Links { get; set; }
            public List<BaseTrack> Items { get; set; }
        }
    }
}