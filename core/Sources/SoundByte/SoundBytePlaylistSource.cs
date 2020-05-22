using JetBrains.Annotations;
using SoundByte.Core.Services;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SoundByte.Core.Sources.SoundByte
{
    
    public class SoundBytePlaylistSource : ISource
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
                        Resources.Resources.Sources_SoundByte_Playlist_NoAccount_Description));
            }

            return await Task.Run(() =>
                new SourceResponse(null, null, false,
                    "Under Development",
                    "This is still under development"));
        }
    }
}