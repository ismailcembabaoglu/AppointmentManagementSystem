using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using AppointmentManagementSystem.Application.Features.Payments.Commands;
using AppointmentManagementSystem.Application.Features.Payments.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
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
        public async Task<IActionResult> PaymentWebhook([FromForm] ProcessPaymentWebhookCommand? posted = null)
        {
            try
            {
                _logger.LogInformation("=== PayTR Webhook Received ===");
                _logger.LogInformation($"Content-Type: {Request.ContentType}");
                _logger.LogInformation($"Method: {Request.Method}");
                _logger.LogInformation($"Request URL: {Request.Scheme}://{Request.Host}{Request.Path}{Request.QueryString}");

                // Buffer and capture the raw body for diagnostics, then rewind so we can parse it again.
                Request.EnableBuffering();
                string rawBody;
                using (var reader = new StreamReader(Request.Body, Encoding.UTF8, leaveOpen: true))
                {
                    rawBody = await reader.ReadToEndAsync();
                }
                Request.Body.Position = 0;

                _logger.LogInformation($"Raw body: {rawBody}");

                IFormCollection? form = null;
                Dictionary<string, Microsoft.Extensions.Primitives.StringValues>? bodyValues = null;

                try
                {
                    // Read form data even if the content type is not flagged correctly; PayTR posts url-encoded data.
                    form = await Request.ReadFormAsync();
                    _logger.LogInformation($"Form count: {form.Count}; keys: {string.Join(", ", form.Keys)}");
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Failed to read Request.Form");
                }

                if (string.IsNullOrWhiteSpace(rawBody) == false)
                {
                    try
                    {
                        bodyValues = QueryHelpers.ParseQuery(rawBody)
                            .ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogDebug(ex, "Raw body is not standard url-encoded");
                    }

                    if (bodyValues == null || bodyValues.Count == 0)
                    {
                        try
                        {
                            var jsonDoc = JsonDocument.Parse(rawBody);
                            bodyValues = jsonDoc.RootElement
                                .EnumerateObject()
                                .ToDictionary(p => p.Name, p => new Microsoft.Extensions.Primitives.StringValues(p.Value.ToString()));
                        }
                        catch (Exception ex)
                        {
                            _logger.LogDebug(ex, "Raw body is not JSON");
                        }
                    }

                    if ((form == null || form.Count == 0) && bodyValues == null)
                    {
                        try
                        {
                            var reconstructed = await new FormReader(rawBody).ReadFormAsync();
                            if (reconstructed.Count > 0)
                            {
                                form = new FormCollection(reconstructed);
                                _logger.LogInformation($"Form reconstructed from raw body; keys: {string.Join(", ", reconstructed.Keys)}");
                            }
                        }
                        catch (Exception ex)
                        {
                            _logger.LogDebug(ex, "Failed to reconstruct form from raw body");
                        }
                    }
                }

                if (bodyValues != null)
                {
                    _logger.LogInformation($"Parsed body keys: {string.Join(", ", bodyValues.Keys)}");
                }

                string? ReadValue(string key)
                {
                    if (form != null && form.TryGetValue(key, out var formValue))
                    {
                        return formValue.ToString();
                    }

                    if (bodyValues != null && bodyValues.TryGetValue(key, out var bodyValue))
                    {
                        return bodyValue.ToString();
                    }

                    if (Request.Query.ContainsKey(key))
                    {
                        return Request.Query[key].ToString();
                    }

                    if (posted != null)
                    {
                        return key switch
                        {
                            "merchant_oid" => posted.MerchantOid,
                            "status" => posted.Status,
                            "total_amount" => posted.TotalAmount,
                            "hash" => posted.Hash,
                            "utoken" => posted.Utoken,
                            "ctoken" => posted.Ctoken,
                            "card_type" => posted.CardType,
                            "masked_pan" => posted.MaskedPan,
                            "payment_id" => posted.PaymentId,
                            "failed_reason_msg" => posted.FailedReasonMsg,
                            "card_number_last_four" => null, // iFrame API'de yok, form'dan gelebilir
                            _ => null
                        };
                    }

                    return null;
                }

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
                _logger.LogInformation($"Utoken: {command.Utoken ?? "NULL"}");
                _logger.LogInformation($"Ctoken: {command.Ctoken ?? "NULL"}");
                _logger.LogInformation($"CardType: {command.CardType ?? "NULL"}");
                _logger.LogInformation($"MaskedPan: {command.MaskedPan ?? "NULL"}");
                _logger.LogInformation($"PaymentId: {command.PaymentId ?? "NULL"}");
                _logger.LogInformation($"FailedReasonMsg: {command.FailedReasonMsg ?? "NULL"}");

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
            _logger.LogInformation("=== Success Redirect Called ===");
            _logger.LogInformation($"Method: {Request.Method}");
            _logger.LogInformation($"Content-Type: {Request.ContentType}");
            _logger.LogInformation($"Query Params - merchant_oid: {merchant_oid}, status: {status}, utoken: {utoken}, ctoken: {ctoken}");
            
            if (Request.HasFormContentType)
            {
                _logger.LogInformation($"Form Keys: {string.Join(", ", Request.Form.Keys)}");
                foreach (var key in Request.Form.Keys)
                {
                    _logger.LogInformation($"Form[{key}] = {Request.Form[key]}");
                }
            }
            
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
            _logger.LogInformation("=== Fail Redirect Called ===");
            _logger.LogInformation($"Method: {Request.Method}");
            _logger.LogInformation($"Content-Type: {Request.ContentType}");
            _logger.LogInformation($"Query Params - merchant_oid: {merchant_oid}, status: {status}, failed_reason_msg: {failed_reason_msg}");
            
            if (Request.HasFormContentType)
            {
                _logger.LogInformation($"Form Keys: {string.Join(", ", Request.Form.Keys)}");
                foreach (var key in Request.Form.Keys)
                {
                    _logger.LogInformation($"Form[{key}] = {Request.Form[key]}");
                }
            }
            
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
