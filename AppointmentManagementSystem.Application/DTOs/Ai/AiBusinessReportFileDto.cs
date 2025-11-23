namespace AppointmentManagementSystem.Application.DTOs.Ai
{
    public class AiBusinessReportFileDto
    {
        public string FileName { get; set; } = string.Empty;
        public string ContentType { get; set; } = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
        public string Base64Data { get; set; } = string.Empty;
    }
}
