using JetBrains.Annotations;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SoundByte.Core.Sources.Generic
{
    /// <summary>
    /// Empty source for tracks. This is used when trying to build a SoundByte
    /// collection when using a list of songs and not a model.
    /// </summary>
    
    public class DummyTrackSource : ISource
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
            return await Task.Run(() => new SourceResponse(null, null, false), cancellationToken);
        }
    }
}