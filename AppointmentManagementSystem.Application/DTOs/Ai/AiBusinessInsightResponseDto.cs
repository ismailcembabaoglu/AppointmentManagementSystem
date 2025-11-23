using System.Collections.Generic;

namespace AppointmentManagementSystem.Application.DTOs.Ai
{
    public class AiBusinessInsightResponseDto
    {
        public string Reply { get; set; } = string.Empty;
        public AiBusinessAnalyticsDto? Analytics { get; set; }
    }
}
