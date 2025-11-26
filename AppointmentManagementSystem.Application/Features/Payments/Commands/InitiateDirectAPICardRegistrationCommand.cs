using MediatR;

namespace AppointmentManagementSystem.Application.Features.Payments.Commands
{
    /// <summary>
    /// Direct API ile ilk kayıt ve kart saklama command
    /// </summary>
    public class InitiateDirectAPICardRegistrationCommand : IRequest<Result<InitiateDirectAPICardRegistrationResponse>>
    {
        public int BusinessId { get; set; }
        public string Email { get; set; } = string.Empty;
        public string BusinessName { get; set; } = string.Empty;
        public string OwnerName { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        
        // Kart bilgileri
        public string CardOwner { get; set; } = string.Empty;
        public string CardNumber { get; set; } = string.Empty;
        public string ExpiryMonth { get; set; } = string.Empty;
        public string ExpiryYear { get; set; } = string.Empty;
        public string CVV { get; set; } = string.Empty;
        
        public string UserIp { get; set; } = string.Empty;
    }

    public class InitiateDirectAPICardRegistrationResponse
    {
        public bool Success { get; set; }
        public string? MerchantOid { get; set; }
        public string? Message { get; set; }
        public string? RedirectUrl { get; set; } // 3D Secure varsa
    }
}
