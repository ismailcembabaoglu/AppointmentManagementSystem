namespace AppointmentManagementSystem.Domain.Entities
{
    public class EmployeePhoto : Photo
    {
        public int EmployeeId { get; set; }
        public virtual Employee? Employee { get; set; } = new();
    }
}
