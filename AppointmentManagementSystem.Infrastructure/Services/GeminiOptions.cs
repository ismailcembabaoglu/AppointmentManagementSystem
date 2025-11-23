namespace AppointmentManagementSystem.Infrastructure.Services
{
    public class GeminiOptions
    {
        public string ApiKey { get; set; } = string.Empty;
        public string Model { get; set; } = "gemini-2.5-flash-preview-09-2025";
        public string BaseUrl { get; set; } = "https://generativelanguage.googleapis.com/v1beta";
    }
}
