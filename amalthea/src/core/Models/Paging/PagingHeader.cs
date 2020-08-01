using Newtonsoft.Json;

namespace SoundByte.Core.Models.Paging
{
    public class PagingHeader
    {
        public PagingHeader(int totalItems, int pageNumber, int pageSize, int totalPages)
        {
            TotalItems = totalItems;
            PageNumber = pageNumber;
            PageSize = pageSize;
            TotalPages = totalPages;
        }

        [JsonProperty("total_items")]
        public int TotalItems { get; }

        [JsonProperty("page_number")]
        public int PageNumber { get; }

        [JsonProperty("page_size")]
        public int PageSize { get; }

        [JsonProperty("total_pages")]
        public int TotalPages { get; }

        public string ToJson() => JsonConvert.SerializeObject(this);
    }
}