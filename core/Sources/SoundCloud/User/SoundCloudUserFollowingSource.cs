/* |----------------------------------------------------------------|
 * | Copyright (c) 2017 - 2018 Grid Entertainment                   |
 * | All Rights Reserved                                            |
 * |                                                                |
 * | This source code is to only be used for educational            |
 * | purposes. Distribution of SoundByte source code in             |
 * | any form outside this repository is forbidden. If you          |
 * | would like to contribute to the SoundByte source code, you     |
 * | are welcome.                                                   |
 * |----------------------------------------------------------------|
 */

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
    
    public class SoundCloudUserFollowingSource : ISource
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
            var followings = await SoundByteService.Current.GetAsync<UserListHolder>(ServiceTypes.SoundCloud, $"/users/{UserId}/followings",
                new Dictionary<string, string>
                {
                    {"limit", count.ToString()},
                    {"linked_partitioning", "1"},
                    {"cursor", token}
                }, cancellationToken).ConfigureAwait(false);

            // If there are no users
            if (!followings.Response.Users.Any())
            {
                return new SourceResponse(null, null, false, "No users", "This user is not following anyone.");
            }

            // Parse uri for cursor
            var param = new QueryParameterCollection(followings.Response.NextList);
            var nextToken = param.FirstOrDefault(x => x.Key == "cursor").Value;

            // Convert SoundCloud specific tracks to base tracks
            var baseUsers = new List<BaseSoundByteItem>();
            followings.Response.Users.ForEach(x => baseUsers.Add(new BaseSoundByteItem(x.ToBaseUser())));

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