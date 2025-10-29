namespace AppointmentManagementSystem.Application.DTOs
{
    public class DocumentDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string FileName { get; set; } = string.Empty;
        public string? FilePath { get; set; }
        public string? ContentType { get; set; }
        public long FileSize { get; set; }
        public string? Base64Data { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
