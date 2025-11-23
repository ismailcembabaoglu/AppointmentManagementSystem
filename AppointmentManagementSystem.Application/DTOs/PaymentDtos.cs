namespace AppointmentManagementSystem.Application.DTOs
{
    public class InitiateCardRegistrationDto
    {
        public string Email { get; set; } = string.Empty;
        public string BusinessName { get; set; } = string.Empty;
        public int BusinessId { get; set; }
        public decimal Amount { get; set; } = 700m;
        public string Description { get; set; } = "Aylık Abonelik Ücreti";
        public bool IsCardUpdate { get; set; }
    }

    public class CardRegistrationResponseDto
    {
        public bool Success { get; set; }
        public string? IFrameToken { get; set; }
        public string? IFrameUrl { get; set; }
        public string? ErrorMessage { get; set; }
    }

    public class PaymentWebhookDto
    {
        public string MerchantOid { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string TotalAmount { get; set; } = string.Empty;
        public string Hash { get; set; } = string.Empty;
        public string? Utoken { get; set; }
        public string? Ctoken { get; set; }
        public string? CardType { get; set; }
        public string? MaskedPan { get; set; }
        public string? PaymentId { get; set; }
        public string? FailedReasonMsg { get; set; }
    }

    public class SubscriptionDto
    {
        public int Id { get; set; }
        public int BusinessId { get; set; }
        public string BusinessName { get; set; } = string.Empty;
        public decimal MonthlyAmount { get; set; }
        public string Currency { get; set; } = string.Empty;
        public string SubscriptionStatus { get; set; } = string.Empty;
        public DateTime? NextBillingDate { get; set; }
        public DateTime? LastBillingDate { get; set; }
        public bool IsActive { get; set; }
        public string? CardBrand { get; set; }
        public string? MaskedCardNumber { get; set; }
    }

    public class PaymentDto
    {
        public int Id { get; set; }
        public int BusinessId { get; set; }
        public string MerchantOid { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public string Currency { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public DateTime? PaymentDate { get; set; }
        public int RetryCount { get; set; }
        public string? ErrorMessage { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}