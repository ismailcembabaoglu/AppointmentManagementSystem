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
        private readonly ILogger<PaymentsController> _logger;

        public PaymentsController(IMediator mediator, IConfiguration configuration, ILogger<PaymentsController> logger)
        {
            _mediator = mediator;
            _configuration = configuration;
            _logger = logger;
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
        public async Task<IActionResult> PaymentWebhook()
        {
            try
            {
                _logger.LogInformation("=== PayTR Webhook Received ===");
                _logger.LogInformation($"Content-Type: {Request.ContentType}");
                _logger.LogInformation($"Method: {Request.Method}");

                string? ReadValue(string key) =>
                    Request.HasFormContentType && Request.Form.ContainsKey(key)
                        ? Request.Form[key].ToString()
                        : Request.Query.ContainsKey(key)
                            ? Request.Query[key].ToString()
                            : null;

                var command = new ProcessPaymentWebhookCommand
                {
                    MerchantOid = ReadValue("merchant_oid") ?? string.Empty,
                    Status = ReadValue("status") ?? string.Empty,
                    TotalAmount = ReadValue("total_amount") ?? string.Empty,
                    Hash = ReadValue("hash") ?? string.Empty,
                    Utoken = ReadValue("utoken"),
                    Ctoken = ReadValue("ctoken"),
                    CardType = ReadValue("card_type"),
                    MaskedPan = ReadValue("masked_pan"),
                    PaymentId = ReadValue("payment_id"),
                    FailedReasonMsg = ReadValue("failed_reason_msg")
                };

                _logger.LogInformation($"MerchantOid: {command.MerchantOid}");
                _logger.LogInformation($"Status: {command.Status}");
                _logger.LogInformation($"TotalAmount: {command.TotalAmount}");

                var result = await _mediator.Send(command);

                // PayTR'ye her durumda "OK" döndürmeliyiz
                // Aksi takdirde PayTR webhook'u başarısız sayar ve tekrar dener
                // Hata durumlarını logluyoruz ama "OK" dönüyoruz
                if (!result.Success)
                {
                    _logger.LogWarning($"Webhook processing failed: {result.Message}");
                }

                _logger.LogInformation("Webhook response: OK");
                return Content("OK");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Critical error in webhook endpoint");
                // PayTR'ye yine de "OK" döndür ki tekrar denemesin
                return Content("OK");
            }
        }

        [HttpGet("success-redirect"), HttpPost("success-redirect")]
        [AllowAnonymous]
        public IActionResult SuccessRedirect(string? merchant_oid, string? status, string? total_amount,
            string? hash, string? utoken, string? ctoken, string? card_type, string? masked_pan,
            string? payment_id, string? failed_reason_msg)
        {
            return BuildRedirect(_configuration["PayTR:SuccessUrl"] ?? "/payment/success",
                merchant_oid, status, total_amount, hash, utoken, ctoken, card_type, masked_pan, payment_id,
                failed_reason_msg);
        }

        [HttpGet("fail-redirect"), HttpPost("fail-redirect")]
        [AllowAnonymous]
        public IActionResult FailRedirect(string? merchant_oid, string? status, string? total_amount,
            string? hash, string? utoken, string? ctoken, string? card_type, string? masked_pan,
            string? payment_id, string? failed_reason_msg)
        {
            return BuildRedirect(_configuration["PayTR:FailUrl"] ?? "/payment/failed",
                merchant_oid, status, total_amount, hash, utoken, ctoken, card_type, masked_pan, payment_id,
                failed_reason_msg);
        }

        private IActionResult BuildRedirect(string destinationUrl, string? merchantOid, string? status,
            string? totalAmount, string? hash, string? utoken, string? ctoken, string? cardType,
            string? maskedPan, string? paymentId, string? failedReasonMsg)
        {
            // Prefer form values if present (PayTR sends POST) but fall back to query string for GET requests.
            string? ReadValue(string key, string? provided) =>
                Request.HasFormContentType && Request.Form.ContainsKey(key)
                    ? Request.Form[key].ToString()
                    : provided;

            var queryParams = new Dictionary<string, string?>
            {
                { "merchant_oid", ReadValue("merchant_oid", merchantOid) },
                { "status", ReadValue("status", status) },
                { "total_amount", ReadValue("total_amount", totalAmount) },
                { "hash", ReadValue("hash", hash) },
                { "utoken", ReadValue("utoken", utoken) },
                { "ctoken", ReadValue("ctoken", ctoken) },
                { "card_type", ReadValue("card_type", cardType) },
                { "masked_pan", ReadValue("masked_pan", maskedPan) },
                { "payment_id", ReadValue("payment_id", paymentId) },
                { "failed_reason_msg", ReadValue("failed_reason_msg", failedReasonMsg) }
            };

            var redirectUrl = QueryHelpers.AddQueryString(destinationUrl, queryParams!);
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
