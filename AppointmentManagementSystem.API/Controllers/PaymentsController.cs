using AppointmentManagementSystem.Application.Features.Payments.Commands;
using AppointmentManagementSystem.Application.Features.Payments.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Configuration;

namespace AppointmentManagementSystem.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentsController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly IConfiguration _configuration;

        public PaymentsController(IMediator mediator, IConfiguration configuration)
        {
            _mediator = mediator;
            _configuration = configuration;
        }

        [HttpPost("initiate-card-registration")]
        [AllowAnonymous]
        public async Task<IActionResult> InitiateCardRegistration([FromBody] InitiateCardRegistrationCommand command)
        {
            // Get user IP
            var userIp = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "127.0.0.1";
            command.UserIp = userIp;

            var result = await _mediator.Send(command);
            
            if (result.Success)
                return Ok(result);
            
            return BadRequest(result);
        }

        [HttpGet("subscription/{businessId}")]
        [Authorize]
        public async Task<IActionResult> GetSubscription(int businessId)
        {
            var query = new GetSubscriptionByBusinessIdQuery { BusinessId = businessId };
            var result = await _mediator.Send(query);
            
            if (result.Success)
                return Ok(result);
            
            return NotFound(result);
        }

        [HttpGet("history/{businessId}")]
        [Authorize]
        public async Task<IActionResult> GetPaymentHistory(int businessId)
        {
            var query = new GetPaymentHistoryQuery { BusinessId = businessId };
            var result = await _mediator.Send(query);
            
            if (result.Success)
                return Ok(result);
            
            return BadRequest(result);
        }

        [HttpPost("webhook")]
        [AllowAnonymous]
        public async Task<IActionResult> PaymentWebhook([FromForm] string merchant_oid, [FromForm] string status,
            [FromForm] string total_amount, [FromForm] string hash, [FromForm] string? utoken,
            [FromForm] string? ctoken, [FromForm] string? card_type, [FromForm] string? masked_pan,
            [FromForm] string? payment_id, [FromForm] string? failed_reason_msg)
        {
            var command = new ProcessPaymentWebhookCommand
            {
                MerchantOid = merchant_oid ?? "",
                Status = status ?? "",
                TotalAmount = total_amount ?? "",
                Hash = hash ?? "",
                Utoken = utoken,
                Ctoken = ctoken,
                CardType = card_type,
                MaskedPan = masked_pan,
                PaymentId = payment_id,
                FailedReasonMsg = failed_reason_msg
            };

            var result = await _mediator.Send(command);
            
            if (result.Success)
                return Ok("OK");

            return BadRequest(result.Message);
        }

        [HttpPost("success-redirect")]
        [AllowAnonymous]
        public IActionResult SuccessRedirect([FromForm] string merchant_oid, [FromForm] string status,
            [FromForm] string total_amount, [FromForm] string hash, [FromForm] string? utoken,
            [FromForm] string? ctoken, [FromForm] string? card_type, [FromForm] string? masked_pan,
            [FromForm] string? payment_id, [FromForm] string? failed_reason_msg)
        {
            var successUrl = _configuration["PayTR:SuccessUrl"] ?? "/payment/success";

            var queryParams = new Dictionary<string, string?>
            {
                { "merchant_oid", merchant_oid },
                { "status", status },
                { "total_amount", total_amount },
                { "hash", hash },
                { "utoken", utoken },
                { "ctoken", ctoken },
                { "card_type", card_type },
                { "masked_pan", masked_pan },
                { "payment_id", payment_id },
                { "failed_reason_msg", failed_reason_msg }
            };

            var redirectUrl = QueryHelpers.AddQueryString(successUrl, queryParams!);
            return Redirect(redirectUrl);
        }

        [HttpPost("fail-redirect")]
        [AllowAnonymous]
        public IActionResult FailRedirect([FromForm] string merchant_oid, [FromForm] string status,
            [FromForm] string total_amount, [FromForm] string hash, [FromForm] string? utoken,
            [FromForm] string? ctoken, [FromForm] string? card_type, [FromForm] string? masked_pan,
            [FromForm] string? payment_id, [FromForm] string? failed_reason_msg)
        {
            var failUrl = _configuration["PayTR:FailUrl"] ?? "/payment/failed";

            var queryParams = new Dictionary<string, string?>
            {
                { "merchant_oid", merchant_oid },
                { "status", status },
                { "total_amount", total_amount },
                { "hash", hash },
                { "utoken", utoken },
                { "ctoken", ctoken },
                { "card_type", card_type },
                { "masked_pan", masked_pan },
                { "payment_id", payment_id },
                { "failed_reason_msg", failed_reason_msg }
            };

            var redirectUrl = QueryHelpers.AddQueryString(failUrl, queryParams!);
            return Redirect(redirectUrl);
        }

        [HttpPost("complete-card-registration")]
        [AllowAnonymous]
        public async Task<IActionResult> CompleteCardRegistration([FromBody] CompleteCardRegistrationCommand command)
        {
            var result = await _mediator.Send(command);

            if (result.Success)
                return Ok(result);

            return BadRequest(result);
        }
    }
}
