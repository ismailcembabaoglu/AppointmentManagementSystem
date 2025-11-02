namespace AppointmentManagementSystem.Application.DTOs
{
    public class AuthResponseDto
    {
        public string Token { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public DateTime ExpiresAt { get; set; }
        public int? BusinessId { get; set; } // Business registration için
    }
}
