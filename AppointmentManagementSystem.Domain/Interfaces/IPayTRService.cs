namespace AppointmentManagementSystem.Domain.Interfaces
{
    public interface IPayTRService
    {
        Task<PayTRCardRegistrationResult> InitiateCardRegistrationAsync(string customerEmail, string userIp, string merchantOid);
        Task<PayTRPaymentResult> ChargeRecurringPaymentAsync(string customerEmail, string utoken, string ctoken, string merchantOid, decimal amount, string userIp);
        Task<PayTRStatusResult> QueryPaymentStatusAsync(string merchantOid);
        string GeneratePayTRToken(string merchantId, string userIp, string merchantOid, string email, string paymentAmount, string currency, int testMode, int non3d, string merchantSalt, string merchantKey);
        string ValidateWebhookSignature(string merchantOid, string status, string totalAmount, string merchantSalt);
    }

    public class PayTRCardRegistrationResult
    {
        public bool Success { get; set; }
        public string? IFrameToken { get; set; }
        public string? IFrameUrl { get; set; }
        public string? ErrorMessage { get; set; }
    }

    public class PayTRPaymentResult
    {
        public bool Success { get; set; }
        public string? Status { get; set; }
        public string? TransactionId { get; set; }
        public string? ErrorMessage { get; set; }
        public string? RawResponse { get; set; }
    }

    public class PayTRStatusResult
    {
        public bool Success { get; set; }
        public string? Status { get; set; }
        public string? ErrorMessage { get; set; }
        public Dictionary<string, object>? Data { get; set; }
    }
}