using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SoundByte.Core.Services.Definitions
{
    public interface ISearchPreviewService
    {
        /// <summary>
        ///     Get a list of search suggestions matching the passed in query
        /// </summary>
        /// <param name="query">The query to search for</param>
        /// <param name="cancellationToken">Cancel running task</param>
        /// <returns>A list of similar tracks that the user might be wanting</returns>
        Task<IEnumerable<string>> GetSearchSuggestionsAsync(string query, CancellationToken cancellationToken = default);
    }
}