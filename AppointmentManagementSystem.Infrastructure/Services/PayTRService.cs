using AppointmentManagementSystem.Domain.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace AppointmentManagementSystem.Infrastructure.Services
{
    public class PayTRService : IPayTRService
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<PayTRService> _logger;
        private readonly HttpClient _httpClient;
        private readonly string _merchantId;
        private readonly string _merchantKey;
        private readonly string _merchantSalt;
        private readonly string _apiUrl;
        private readonly bool _isTestMode;

        public PayTRService(IConfiguration configuration, ILogger<PayTRService> logger, HttpClient httpClient)
        {
            _configuration = configuration;
            _logger = logger;
            _httpClient = httpClient;
            _httpClient.Timeout = TimeSpan.FromSeconds(30);

            _merchantId = _configuration["PayTR:MerchantId"] ?? "sandbox-merchant-id";
            _merchantKey = _configuration["PayTR:MerchantKey"] ?? "sandbox-merchant-key";
            _merchantSalt = _configuration["PayTR:MerchantSalt"] ?? "sandbox-merchant-salt";
            _apiUrl = _configuration["PayTR:ApiUrl"] ?? "https://www.paytr.com/odeme";
            _isTestMode = _configuration.GetValue<bool>("PayTR:TestMode", true);
        }

        public async Task<PayTRCardRegistrationResult> InitiateCardRegistrationAsync(string customerEmail, string userIp, string merchantOid)
        {
            try
            {
                var token = GeneratePayTRToken(
                    _merchantId,
                    userIp,
                    merchantOid,
                    customerEmail,
                    "0", // Card registration doesn't charge
                    "TRY",
                    _isTestMode ? 1 : 0,
                    0, // 3D Secure for card registration
                    _merchantSalt,
                    _merchantKey
                );

                var parameters = new Dictionary<string, string>
                {
                    { "merchant_id", _merchantId },
                    { "user_ip", userIp },
                    { "merchant_oid", merchantOid },
                    { "email", customerEmail },
                    { "payment_amount", "0" },
                    { "payment_type", "card" },
                    { "installment_count", "0" },
                    { "currency", "TRY" },
                    { "test_mode", _isTestMode ? "1" : "0" },
                    { "non_3d", "0" },
                    { "store_card", "1" }, // Store card for future use
                    { "paytr_token", token },
                    { "post_url", _apiUrl } // PayTR iframe için gerekli
                };

                _logger.LogInformation($"Card registration initiated for {customerEmail}, MerchantOid: {merchantOid}");

                return new PayTRCardRegistrationResult
                {
                    Success = true,
                    IFrameToken = token,
                    IFrameParameters = parameters
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error initiating card registration for {customerEmail}");
                return new PayTRCardRegistrationResult
                {
                    Success = false,
                    ErrorMessage = ex.Message
                };
            }
        }

        public async Task<PayTRPaymentResult> ChargeRecurringPaymentAsync(
            string customerEmail,
            string utoken,
            string ctoken,
            string merchantOid,
            decimal amount,
            string userIp)
        {
            try
            {
                var amountStr = ((int)(amount * 100)).ToString(); // Convert to kuruş

                var token = GeneratePayTRToken(
                    _merchantId,
                    userIp,
                    merchantOid,
                    customerEmail,
                    amountStr,
                    "TRY",
                    _isTestMode ? 1 : 0,
                    1, // Non-3D for recurring
                    _merchantSalt,
                    _merchantKey
                );

                var content = new FormUrlEncodedContent(new Dictionary<string, string>
                {
                    { "merchant_id", _merchantId },
                    { "user_ip", userIp },
                    { "merchant_oid", merchantOid },
                    { "email", customerEmail },
                    { "payment_amount", amountStr },
                    { "payment_type", "card" },
                    { "installment_count", "0" },
                    { "currency", "TRY" },
                    { "test_mode", _isTestMode ? "1" : "0" },
                    { "non_3d", "1" },
                    { "recurring_payment", "1" },
                    { "utoken", utoken },
                    { "ctoken", ctoken },
                    { "paytr_token", token }
                });

                var response = await _httpClient.PostAsync(_apiUrl, content);
                var responseText = await response.Content.ReadAsStringAsync();

                _logger.LogInformation($"PayTR recurring payment response for {merchantOid}: {response.StatusCode}");

                if (response.IsSuccessStatusCode)
                {
                    var jsonResponse = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(responseText);
                    var status = jsonResponse?["status"].GetString();

                    return new PayTRPaymentResult
                    {
                        Success = status == "success",
                        Status = status,
                        TransactionId = jsonResponse?.ContainsKey("payment_id") == true ? jsonResponse["payment_id"].GetString() : null,
                        RawResponse = responseText
                    };
                }
                else
                {
                    return new PayTRPaymentResult
                    {
                        Success = false,
                        ErrorMessage = $"HTTP {response.StatusCode}",
                        RawResponse = responseText
                    };
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error charging recurring payment for {merchantOid}");
                return new PayTRPaymentResult
                {
                    Success = false,
                    ErrorMessage = ex.Message
                };
            }
        }

        public async Task<PayTRStatusResult> QueryPaymentStatusAsync(string merchantOid)
        {
            try
            {
                var statusUrl = _configuration["PayTR:StatusUrl"] ?? "https://www.paytr.com/odeme/durum-sorgu";

                var content = new FormUrlEncodedContent(new Dictionary<string, string>
                {
                    { "merchant_id", _merchantId },
                    { "merchant_key", _merchantKey },
                    { "merchant_oid", merchantOid }
                });

                var response = await _httpClient.PostAsync(statusUrl, content);
                var responseText = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    var jsonResponse = JsonSerializer.Deserialize<Dictionary<string, object>>(responseText);
                    
                    return new PayTRStatusResult
                    {
                        Success = true,
                        Data = jsonResponse
                    };
                }
                else
                {
                    return new PayTRStatusResult
                    {
                        Success = false,
                        ErrorMessage = $"HTTP {response.StatusCode}"
                    };
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error querying payment status for {merchantOid}");
                return new PayTRStatusResult
                {
                    Success = false,
                    ErrorMessage = ex.Message
                };
            }
        }

        public string GeneratePayTRToken(
            string merchantId,
            string userIp,
            string merchantOid,
            string email,
            string paymentAmount,
            string currency,
            int testMode,
            int non3d,
            string merchantSalt,
            string merchantKey)
        {
            var hashStr = $"{merchantId}{userIp}{merchantOid}{email}{paymentAmount}card0{currency}{testMode}{non3d}";
            var paytrToken = hashStr + merchantSalt;

            using var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(merchantKey));
            var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(paytrToken));
            return Convert.ToBase64String(hash);
        }

        public string ValidateWebhookSignature(string merchantOid, string status, string totalAmount, string merchantSalt)
        {
            var hashString = $"{merchantOid}{merchantSalt}{status}{totalAmount}";
            using var md5 = MD5.Create();
            var hash = md5.ComputeHash(Encoding.UTF8.GetBytes(hashString));
            return BitConverter.ToString(hash).Replace("-", "").ToLower();
        }
    }
}
