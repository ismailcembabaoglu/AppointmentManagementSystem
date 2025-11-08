using AppointmentManagementSystem.Application.DTOs;
using MediatR;

namespace AppointmentManagementSystem.Application.Features.Admin.Queries
{
    public class GetBusinessDetailAdminQuery : IRequest<BusinessDetailAdminDto>
    {
        public int BusinessId { get; set; }
    }

    public class BusinessDetailAdminDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public string? Address { get; set; }
        public string? City { get; set; }
        public string? District { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public string? CategoryName { get; set; }
        
        // Statistics
        public int TotalAppointments { get; set; }
        public int CompletedAppointments { get; set; }
        public int CancelledAppointments { get; set; }
        public int TotalEmployees { get; set; }
        public int TotalServices { get; set; }
        public decimal? AverageRating { get; set; }
        public int TotalReviews { get; set; }
        
        // Subscription
        public string? SubscriptionStatus { get; set; }
        public DateTime? SubscriptionStartDate { get; set; }
        public DateTime? NextBillingDate { get; set; }
        public string? CardLastFourDigits { get; set; }
        public string? CardType { get; set; }
        public bool AutoRenewal { get; set; }
    }
}