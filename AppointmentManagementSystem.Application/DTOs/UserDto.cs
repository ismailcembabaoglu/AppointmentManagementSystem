namespace AppointmentManagementSystem.Application.DTOs
{
    public class UserDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public string? PhoneNumber { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string? ProfilePhotoPath { get; set; }
        public string? Address { get; set; }
        public string? City { get; set; }
        public int? OwnedBusinessId { get; set; }
        public bool IsActive { get; set; }
    }
}
