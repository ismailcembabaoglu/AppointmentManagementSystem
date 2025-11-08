namespace AppointmentManagementSystem.BlazorUI.Models
{
    public class BusinessAdminModel
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? CategoryName { get; set; }
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public int TotalAppointments { get; set; }
        public int TotalEmployees { get; set; }
        public decimal? AverageRating { get; set; }
        public int TotalReviews { get; set; }
        public string? SubscriptionStatus { get; set; }
        public DateTime? NextBillingDate { get; set; }
    }
}