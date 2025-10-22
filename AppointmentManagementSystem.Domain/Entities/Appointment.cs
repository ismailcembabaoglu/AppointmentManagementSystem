using System.ComponentModel.DataAnnotations;

namespace AppointmentManagementSystem.Domain.Entities
{
    public class Appointment : BaseEntity
    {
        [Required]
        public int CustomerId { get; set; } // User.Id (Customer rolünde)

        [Required]
        public int BusinessId { get; set; }

        public int? EmployeeId { get; set; }

        [Required]
        public int ServiceId { get; set; }

        [Required]
        public DateTime AppointmentDate { get; set; }

        [Required]
        public TimeSpan StartTime { get; set; }

        public TimeSpan EndTime { get; set; }

        [MaxLength(1000)]
        public string? Notes { get; set; }

        [Required]
        [MaxLength(50)]
        public string Status { get; set; } = "Pending"; // Pending, Confirmed, Cancelled, Completed

        public int? CreatedByBusinessUserId { get; set; } // Hangi şirket kullanıcısı oluşturdu

        // Rating
        public int? Rating { get; set; } // 1-5 yıldız
        public string? Review { get; set; }
        public DateTime? RatingDate { get; set; }

        // Navigation properties
        public virtual User Customer { get; set; } = new();
        public virtual Business Business { get; set; } = new();
        public virtual Employee? Employee { get; set; }
        public virtual Service Service { get; set; } = new();
        public virtual BusinessUser? CreatedBy { get; set; }
        public virtual ICollection<AppointmentPhoto> Photos { get; set; } = new List<AppointmentPhoto>();
    }
}
