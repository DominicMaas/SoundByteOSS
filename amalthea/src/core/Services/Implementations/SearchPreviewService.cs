using Flurl.Http;
using SoundByte.Core.Services.Definitions;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SoundByte.Core.Services.Implementations
{
    public class SearchPreviewService : ISearchPreviewService
    {
        public async Task<IEnumerable<string>> GetSearchSuggestionsAsync(string query, CancellationToken cancellationToken = default)
        {
            var url = $"https://ws.audioscrobbler.com/2.0/?method=track.search&track={query}&api_key={Constants.LastFmApiKey}&format=json";
            var result = await url.GetJsonAsync<SearchQuery>(cancellationToken);
            if (result.Results?.TrackMatches?.Track == null)
                return new List<string>();

            var suggestionList = new List<string>();
            foreach (var t in result.Results.TrackMatches.Track)
            {
                if (t.Name != null)
                    suggestionList.Add(t.Name);
            }

            return suggestionList;
        }

        private class Track
        {
            public string? Name { get; set; }
            public string? Artist { get; set; }
        }

        private class Trackmatches
        {
            public List<Track>? Track { get; set; }
        }

        private class SearchResults
        {
            public Trackmatches? TrackMatches { get; set; }
        }

        private class SearchQuery
        {
            public SearchResults? Results { get; set; }
        }
    }
}