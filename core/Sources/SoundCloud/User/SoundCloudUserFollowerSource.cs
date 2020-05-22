using JetBrains.Annotations;
using Newtonsoft.Json;
using SoundByte.Core.Helpers;
using SoundByte.Core.Items.Generic;
using SoundByte.Core.Items.User;
using SoundByte.Core.Services;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SoundByte.Core.Sources.SoundCloud.User
{
    
    public class SoundCloudUserFollowerSource : ISource
    {
        public string UserId { get; set; }

        public override Dictionary<string, object> GetParameters()
        {
            return new Dictionary<string, object>
            {
                { "i", UserId },
            };
        }

        public override void ApplyParameters(Dictionary<string, object> data)
        {
            data.TryGetValue("u", out var userId);
            UserId = userId.ToString();
        }

        public override async Task<SourceResponse> GetItemsAsync(int count, string token,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            // Call the SoundCloud API and get the items
            var followers = await SoundByteService.Current.GetAsync<UserListHolder>(ServiceTypes.SoundCloud, $"/users/{UserId}/followers",
                new Dictionary<string, string>
                {
                    {"limit", count.ToString()},
                    {"linked_partitioning", "1"},
                    {"cursor", token}
                }, cancellationToken).ConfigureAwait(false);

            // If there are no users
            if (!followers.Response.Users.Any())
            {
                return new SourceResponse(null, null, false, "No users", "This user has no followers.");
            }

            // Parse uri for cursor
            var param = new QueryParameterCollection(followers.Response.NextList);
            var nextToken = param.FirstOrDefault(x => x.Key == "cursor").Value;

            // Convert SoundCloud specific tracks to base tracks
            var baseUsers = new List<BaseSoundByteItem>();
            followers.Response.Users.ForEach(x => baseUsers.Add(new BaseSoundByteItem(x.ToBaseUser())));

            // Return the items
            return new SourceResponse(baseUsers, nextToken);
        }

        [JsonObject]
        private class UserListHolder
        {
            /// <summary>
            ///     List of users
            /// </summary>
            [JsonProperty("collection")]
            public List<SoundCloudUser> Users { get; set; }

            /// <summary>
            ///     The next list of items
            /// </summary>
            [JsonProperty("next_href")]
            public string NextList { get; set; }
        }
    }
}