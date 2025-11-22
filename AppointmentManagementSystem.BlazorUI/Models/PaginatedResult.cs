using System.Collections.Generic;

namespace AppointmentManagementSystem.BlazorUI.Models
{
    public class PaginatedResult<T>
    {
        public List<T> Items { get; set; } = new();
        public int TotalCount { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public Dictionary<string, int> StatusCounts { get; set; } = new();
    }
}
