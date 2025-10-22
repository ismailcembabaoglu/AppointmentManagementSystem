using System.ComponentModel.DataAnnotations;

namespace AppointmentManagementSystem.Application.DTOs
{
    public class AddAppointmentRatingDto
    {
        [Required]
        [Range(1, 5)]
        public int Rating { get; set; }

        [MaxLength(1000)]
        public string? Review { get; set; }
    }
}
