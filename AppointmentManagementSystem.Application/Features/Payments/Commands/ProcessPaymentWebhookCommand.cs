using AppointmentManagementSystem.Application.Shared;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace AppointmentManagementSystem.Application.Features.Payments.Commands
{
    public class ProcessPaymentWebhookCommand : IRequest<Result<bool>>
    {
        [FromForm(Name = "merchant_oid")]
        public string MerchantOid { get; set; } = string.Empty;
        
        [FromForm(Name = "status")]
        public string Status { get; set; } = string.Empty;
        
        [FromForm(Name = "total_amount")]
        public string TotalAmount { get; set; } = string.Empty;
        
        [FromForm(Name = "hash")]
        public string Hash { get; set; } = string.Empty;
        
        [FromForm(Name = "utoken")]
        public string? Utoken { get; set; }
        
        [FromForm(Name = "ctoken")]
        public string? Ctoken { get; set; }
        
        [FromForm(Name = "card_type")]
        public string? CardType { get; set; }
        
        [FromForm(Name = "masked_pan")]
        public string? MaskedPan { get; set; }
        
        [FromForm(Name = "payment_id")]
        public string? PaymentId { get; set; }
        
        [FromForm(Name = "failed_reason_msg")]
        public string? FailedReasonMsg { get; set; }
    }
}