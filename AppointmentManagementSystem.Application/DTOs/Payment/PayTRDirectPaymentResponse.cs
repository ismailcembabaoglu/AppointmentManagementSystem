namespace AppointmentManagementSystem.Application.DTOs.Payment
{
    public class PayTRDirectPaymentResponse
    {
        public bool Success { get; set; }
        public string? Status { get; set; }
        public string? Reason { get; set; }
        public string? PaymentUrl { get; set; } // 3D Secure için redirect URL
        public string? MerchantOid { get; set; }
        public string? ErrorMessage { get; set; }
        public string? UserToken { get; set; }
        public string? CardToken { get; set; }
        public string? MaskedPan { get; set; }
        public string? CardBrand { get; set; }
    }

    public class PayTRCardListResponse
    {
        public bool Success { get; set; }
        public string? Status { get; set; }
        public List<PayTRStoredCard>? Cards { get; set; }
        public string? ErrorMessage { get; set; }
    }

    public class PayTRStoredCard
    {
        public string? Ctoken { get; set; }
        public string? CardBrand { get; set; } // Visa, Mastercard, etc.
        public string? CardAssociation { get; set; }
        public string? MaskedPan { get; set; } // **** **** **** 1234
        public string? ExpiryMonth { get; set; }
        public string? ExpiryYear { get; set; }
        public bool RequireCvv { get; set; } // CVV gerekli mi?
    }
}
