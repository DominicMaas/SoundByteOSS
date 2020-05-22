using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SoundByte.Core.Sources
{
    /// <summary>
    ///     This interface represents an incremental data source. This data layer connected
    ///     into the UI layer for each platform.
    /// </summary>
    public abstract class ISource
    {
        /// <summary>
        ///     Get parameters for the current source.
        /// </summary>
        public abstract Dictionary<string, object> GetParameters();

        /// <summary>
        ///     Apply parameters to the current source.
        /// </summary>
        public abstract void ApplyParameters(Dictionary<string, object> data);

        /// <summary>
        ///     This method returns a list of items depending on the count and token
        /// </summary>
        /// <param name="count">A amount of items to get from the API</param>
        /// <param name="token">A Token place holder</param>
        /// <param name="cancellationToken"></param>
        /// <returns>Returns a collection of BaseSoundByteItem's.</returns>
        public abstract Task<SourceResponse> GetItemsAsync(int count, string token, CancellationToken cancellationToken = default(CancellationToken));
    }
}