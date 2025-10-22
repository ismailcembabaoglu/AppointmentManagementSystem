namespace AppointmentManagementSystem.Application.DTOs
{
    public class BusinessUserDto
    {
        public int Id { get; set; }
        public int BusinessId { get; set; }
        public string BusinessName { get; set; } = string.Empty;
        public int UserId { get; set; }
        public string UserName { get; set; } = string.Empty;
        public string? Position { get; set; }
        public string? Description { get; set; }
        public bool IsActive { get; set; }
        public bool CanCreateAppointments { get; set; }
        public bool CanManageServices { get; set; }
        public bool CanManageEmployees { get; set; }
    }
}
