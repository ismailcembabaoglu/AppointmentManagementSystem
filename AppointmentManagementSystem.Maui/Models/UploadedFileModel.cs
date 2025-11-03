namespace AppointmentManagementSystem.Maui.Models
{
    public class UploadedFileModel
    {
        public string? FileName { get; set; }
        public string? ContentType { get; set; }
        public byte[]? FileContent { get; set; }
        public long FileSize { get; set; }
        public string? Base64Content { get; set; }
    }
}
