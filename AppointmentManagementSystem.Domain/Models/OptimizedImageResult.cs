namespace AppointmentManagementSystem.Domain.Models
{
    public class OptimizedImageResult
    {
        public string Base64Data { get; set; } = string.Empty;

        public string ContentType { get; set; } = "image/jpeg";

        public long FileSize { get; set; }
    }
}
