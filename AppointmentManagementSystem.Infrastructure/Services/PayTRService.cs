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
                // IPv6 localhost'u IPv4'e çevir
                if (userIp == "::1" || userIp == "::ffff:127.0.0.1")
                {
                    userIp = "127.0.0.1";
                }

                // user_basket oluştur - PayTR zorunlu alan
                var userBasket = new[]
                {
                    new object[] { "Aylık Abonelik Ücreti", "700.00", 1 }
                };
                var userBasketJson = JsonSerializer.Serialize(userBasket);
                var userBasketBase64 = Convert.ToBase64String(Encoding.UTF8.GetBytes(userBasketJson));

                var paymentAmount = "70000"; // 700 TL * 100 = 70000 kuruş
                var noInstallment = "0";
                var maxInstallment = "0";
                var currency = "TRY";
                var testMode = _isTestMode ? "1" : "0";

                // Token oluştur
                var hashStr = $"{_merchantId}{userIp}{merchantOid}{customerEmail}{paymentAmount}{userBasketBase64}{noInstallment}{maxInstallment}{currency}{testMode}";
                var token = GeneratePayTRToken(hashStr, _merchantSalt, _merchantKey);

                // PayTR API'ye POST isteği gönder
                var formData = new Dictionary<string, string>
                {
                    { "merchant_id", _merchantId },
                    { "user_ip", userIp },
                    { "merchant_oid", merchantOid },
                    { "email", customerEmail },
                    { "payment_amount", paymentAmount },
                    { "paytr_token", token },
                    { "user_basket", userBasketBase64 },
                    { "debug_on", "1" },
                    { "no_installment", noInstallment },
                    { "max_installment", maxInstallment },
                    { "user_name", customerEmail }, // Placeholder
                    { "user_address", "Türkiye" }, // Placeholder
                    { "user_phone", "5555555555" }, // Placeholder
                    { "merchant_ok_url", $"{_configuration["PayTR:CallbackUrl"]?.Replace("/webhook", "/success")}" },
                    { "merchant_fail_url", $"{_configuration["PayTR:CallbackUrl"]?.Replace("/webhook", "/fail")}" },
                    { "timeout_limit", "30" },
                    { "currency", currency },
                    { "test_mode", testMode }
                };

                // Debug logging
                _logger.LogInformation($"PayTR Request - MerchantId: {_merchantId}, UserIp: {userIp}, MerchantOid: {merchantOid}");
                _logger.LogInformation($"PayTR Request - Email: {customerEmail}, Amount: {paymentAmount}, Basket: {userBasketBase64}");
                _logger.LogInformation($"PayTR Request - Token: {token.Substring(0, 20)}...");

                var content = new FormUrlEncodedContent(formData);
                var response = await _httpClient.PostAsync("https://www.paytr.com/odeme/api/get-token", content);
                var responseText = await response.Content.ReadAsStringAsync();

                _logger.LogInformation($"PayTR Response Status: {response.StatusCode}");
                _logger.LogInformation($"PayTR Response Body: {responseText}");

                var jsonResponse = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(responseText);
                
                if (jsonResponse != null && jsonResponse.ContainsKey("status") && jsonResponse["status"].GetString() == "success")
                {
                    var iframeToken = jsonResponse["token"].GetString();
                    
                    return new PayTRCardRegistrationResult
                    {
                        Success = true,
                        IFrameToken = iframeToken,
                        IFrameUrl = $"https://www.paytr.com/odeme/guvenli/{iframeToken}"
                    };
                }
                else
                {
                    var reason = jsonResponse?.ContainsKey("reason") == true ? jsonResponse["reason"].GetString() : "Unknown error";
                    _logger.LogError($"PayTR token failed: {reason}");
                    return new PayTRCardRegistrationResult
                    {
                        Success = false,
                        ErrorMessage = reason
                    };
                }
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

                var hashStr = $"{_merchantId}{userIp}{merchantOid}{customerEmail}{amountStr}card0TRY{(_isTestMode ? 1 : 0)}1";
                var token = GeneratePayTRToken(hashStr, _merchantSalt, _merchantKey);

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

        private string GeneratePayTRToken(string hashString, string merchantSalt, string merchantKey)
        {
            var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(merchantKey));
            var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(hashString + merchantSalt));
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
