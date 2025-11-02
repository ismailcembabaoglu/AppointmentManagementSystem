using System.ComponentModel.DataAnnotations;

namespace AppointmentManagementSystem.Domain.Entities
{
    public class Payment : BaseEntity
    {
        [Required]
        public int BusinessId { get; set; }

        [Required]
        [MaxLength(100)]
        public string MerchantOid { get; set; } = string.Empty; // PayTR Order ID

        [Required]
        public decimal Amount { get; set; }

        [Required]
        [MaxLength(10)]
        public string Currency { get; set; } = "TRY";

        [Required]
        [MaxLength(50)]
        public string Status { get; set; } = "Pending"; // Pending, Success, Failed, Cancelled

        public DateTime? PaymentDate { get; set; }

        // Retry logic
        public int RetryCount { get; set; } = 0;
        public DateTime? NextRetryDate { get; set; }
        public int MaxRetries { get; set; } = 5;

        [MaxLength(1000)]
        public string? ErrorMessage { get; set; }

        public string? PayTRResponse { get; set; } // Raw PayTR response

        [MaxLength(50)]
        public string? PayTRTransactionId { get; set; }

        // Navigation properties
        public virtual Business? Business { get; set; }
    }

    public static class PaymentStatus
    {
        public const string Pending = "Pending";
        public const string Success = "Success";
        public const string Failed = "Failed";
        public const string Cancelled = "Cancelled";
        public const string RetryScheduled = "RetryScheduled";
    }
}