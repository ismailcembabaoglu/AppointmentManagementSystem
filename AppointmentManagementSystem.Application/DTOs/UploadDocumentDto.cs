namespace AppointmentManagementSystem.Application.DTOs
{
    /// <summary>
    /// DTO for uploading a document with Base64 encoded data
    /// </summary>
    public class UploadDocumentDto
    {
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string FileName { get; set; } = string.Empty;
        public string Base64Data { get; set; } = string.Empty;
        public string? ContentType { get; set; }
    }
}
