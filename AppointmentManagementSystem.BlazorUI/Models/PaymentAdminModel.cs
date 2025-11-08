namespace AppointmentManagementSystem.BlazorUI.Models
{
    public class PaymentAdminModel
    {
        public int Id { get; set; }
        public int BusinessId { get; set; }
        public string BusinessName { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public string Status { get; set; } = string.Empty;
        public string TransactionId { get; set; } = string.Empty;
        public DateTime PaymentDate { get; set; }
        public string? CardType { get; set; }
        public string? MaskedCardNumber { get; set; }
        public string? PaymentType { get; set; }
        public int? RetryAttempt { get; set; }
        public string? ErrorMessage { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}