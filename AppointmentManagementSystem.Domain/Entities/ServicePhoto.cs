namespace AppointmentManagementSystem.Domain.Entities
{
    public class ServicePhoto : Photo
    {
        public int ServiceId { get; set; }
        public virtual Service? Service { get; set; }
    }
}
