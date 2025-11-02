using System.ComponentModel.DataAnnotations;

namespace AppointmentManagementSystem.Domain.Entities
{
    public class BusinessSubscription : BaseEntity
    {
        [Required]
        public int BusinessId { get; set; }

        // PayTR Tokens
        [MaxLength(200)]
        public string? PayTRUserToken { get; set; } // utoken

        [MaxLength(200)]
        public string? PayTRCardToken { get; set; } // ctoken

        [MaxLength(50)]
        public string? CardBrand { get; set; } // Visa, Mastercard, etc.

        [MaxLength(20)]
        public string? MaskedCardNumber { get; set; } // Last 4 digits

        // Subscription Details
        [Required]
        public decimal MonthlyAmount { get; set; } = 700.00m;

        [MaxLength(10)]
        public string Currency { get; set; } = "TRY";

        [Required]
        [MaxLength(50)]
        public string SubscriptionStatus { get; set; } = "Active"; // Active, Suspended, Cancelled

        public DateTime? NextBillingDate { get; set; }
        public DateTime? LastBillingDate { get; set; }
        public DateTime? SubscriptionStartDate { get; set; }
        public DateTime? SubscriptionEndDate { get; set; }

        public bool IsActive { get; set; } = true;

        // Navigation properties
        public virtual Business? Business { get; set; }
    }

    public static class SubscriptionStatus
    {
        public const string Active = "Active";
        public const string Suspended = "Suspended";
        public const string Cancelled = "Cancelled";
        public const string PendingPayment = "PendingPayment";
    }
}