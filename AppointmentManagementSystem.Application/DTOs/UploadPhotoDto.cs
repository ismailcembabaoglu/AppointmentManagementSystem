namespace AppointmentManagementSystem.Application.DTOs
{
    /// <summary>
    /// DTO for uploading a photo with Base64 encoded data
    /// </summary>
    public class UploadPhotoDto
    {
        public string FileName { get; set; } = string.Empty;
        public string Base64Data { get; set; } = string.Empty;
        public string? ContentType { get; set; }
    }
}
