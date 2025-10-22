namespace AppointmentManagementSystem.Application.DTOs
{
    public class EmployeeDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Specialization { get; set; }
        public string? Description { get; set; }
        public int BusinessId { get; set; }
        public string BusinessName { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public List<string> PhotoUrls { get; set; } = new();
        public List<string> DocumentUrls { get; set; } = new();
    }
}
