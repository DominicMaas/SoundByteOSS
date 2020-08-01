using SoundByte.Core.Models.Sources;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SoundByte.Core.Sources
{
    /// <summary>
    ///     Source that all sources must extend from
    /// </summary>
    public interface ISource
    {
        /// <summary>
        ///     Get parameters for the current source.
        /// </summary>
        Dictionary<string, string> GetParameters();

        /// <summary>
        ///     Apply parameters to the current source.
        /// </summary>
        void ApplyParameters(Dictionary<string, string> data);

        /// <summary>
        ///     This method returns a list of items depending on the count and token
        /// </summary>
        /// <param name="count">A amount of items to get from the API</param>
        /// <param name="token">A Token place holder</param>
        /// <param name="cancellationToken"></param>
        /// <returns>Returns a collection of BaseSoundByteItem's.</returns>
        Task<SourceResponse> GetItemsAsync(int count, string token, CancellationToken cancellationToken = default);
    }
}