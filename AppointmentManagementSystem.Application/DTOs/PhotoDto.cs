namespace AppointmentManagementSystem.Application.DTOs
{
    public class PhotoDto
    {
        public int Id { get; set; }
        public string FileName { get; set; } = string.Empty;
        public string? FilePath { get; set; }
        public string? ContentType { get; set; }
        public long FileSize { get; set; }
        public string? Base64Data { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
