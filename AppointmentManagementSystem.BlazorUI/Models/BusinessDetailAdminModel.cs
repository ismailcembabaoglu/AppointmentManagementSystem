namespace AppointmentManagementSystem.BlazorUI.Models
{
    public class BusinessDetailAdminModel
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public string? Address { get; set; }
        public string? City { get; set; }
        public string? District { get; set; }
        public string? MapEmbedCode { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public string? CategoryName { get; set; }
        
        public int TotalAppointments { get; set; }
        public int CompletedAppointments { get; set; }
        public int CancelledAppointments { get; set; }
        public int TotalEmployees { get; set; }
        public int TotalServices { get; set; }
        public decimal? AverageRating { get; set; }
        public int TotalReviews { get; set; }
        
        public string? SubscriptionStatus { get; set; }
        public DateTime? SubscriptionStartDate { get; set; }
        public DateTime? NextBillingDate { get; set; }
        public string? CardLastFourDigits { get; set; }
        public string? CardType { get; set; }
        public bool AutoRenewal { get; set; }
    }
}