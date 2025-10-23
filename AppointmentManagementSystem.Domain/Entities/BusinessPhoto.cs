namespace AppointmentManagementSystem.Domain.Entities
{
    public class BusinessPhoto : Photo
    {
        public int BusinessId { get; set; }
        public virtual Business? Business { get; set; } = new();
    }
}
