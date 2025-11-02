namespace AppointmentManagementSystem.Domain.Entities
{
    public class AppointmentPhoto : Photo
    {
        public int AppointmentId { get; set; }
        public virtual Appointment? Appointment { get; set; }
    }
}
