using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace AppointmentManagementSystem.Maui.Models
{
    public class PaginatedResult<T>
    {
        [JsonPropertyName("items")]
        public List<T> Items { get; set; } = new();

        [JsonPropertyName("totalCount")]
        public int TotalCount { get; set; }

        [JsonPropertyName("pageNumber")]
        public int PageNumber { get; set; }

        [JsonPropertyName("pageSize")]
        public int PageSize { get; set; }

        [JsonPropertyName("statusCounts")]
        public Dictionary<string, int> StatusCounts { get; set; } = new();
    }
}
