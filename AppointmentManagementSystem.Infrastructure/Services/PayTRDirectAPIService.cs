using AppointmentManagementSystem.Application.DTOs.Payment;
using AppointmentManagementSystem.Application.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace AppointmentManagementSystem.Infrastructure.Services
{
    /// <summary>
    /// PayTR Direct API Service Implementation
    /// Kart saklama ve recurring payment için PayTR Direct API kullanır
    /// </summary>
    public class PayTRDirectAPIService : IPayTRDirectAPIService
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<PayTRDirectAPIService> _logger;
        private readonly IHttpClientFactory _httpClientFactory;
        
        private readonly string _merchantId;
        private readonly string _merchantKey;
        private readonly string _merchantSalt;
        private readonly bool _isTestMode;
        private readonly string _merchantOkUrl;
        private readonly string _merchantFailUrl;
        private readonly string _callbackUrl;

        public PayTRDirectAPIService(
            IConfiguration configuration,
            ILogger<PayTRDirectAPIService> logger,
            IHttpClientFactory httpClientFactory)
        {
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _httpClientFactory = httpClientFactory ?? throw new ArgumentNullException(nameof(httpClientFactory));

            _logger.LogInformation("🔵 PayTRDirectAPIService constructor called");

            _merchantId = _configuration["PayTR:MerchantId"] ?? throw new ArgumentNullException("PayTR:MerchantId");
            _merchantKey = _configuration["PayTR:MerchantKey"] ?? throw new ArgumentNullException("PayTR:MerchantKey");
            _merchantSalt = _configuration["PayTR:MerchantSalt"] ?? throw new ArgumentNullException("PayTR:MerchantSalt");
            _isTestMode = _configuration.GetValue<bool>("PayTR:TestMode");
            
            var frontendUrl = _configuration["PayTR:FrontendUrl"] ?? "https://aptivaplan.com.tr";
            _merchantOkUrl = $"{frontendUrl}/payment/success";
            _merchantFailUrl = $"{frontendUrl}/payment/fail";
            _callbackUrl = _configuration["PayTR:CallbackUrl"] ?? "https://hub.aptivaplan.com.tr/api/payments/webhook";

            _logger.LogInformation($"✅ PayTRDirectAPIService initialized. MerchantId: {_merchantId}, TestMode: {_isTestMode}");
        }

        public async Task<PayTRDirectPaymentResponse> InitiateCardRegistrationPayment(
            int businessId,
            string email,
            string userName,
            string userAddress,
            string userPhone,
            string ccOwner,
            string cardNumber,
            string expiryMonth,
            string expiryYear,
            string cvv,
            decimal amount,
            string merchantOid,
            string userIp,
            string? existingUtoken = null)
        {
            try
            {
                _logger.LogInformation($"🔵 Direct API: Initiating card registration payment for Business {businessId}");
                _logger.LogInformation($"MerchantOid: {merchantOid}, Amount: {amount}, Email: {email}");
                
                if (!string.IsNullOrEmpty(existingUtoken))
                {
                    _logger.LogInformation($"Existing UToken provided: {existingUtoken.Substring(0, Math.Min(10, existingUtoken.Length))}...");
                }

                // Sepet oluştur
                var userBasket = new[]
                {
                    new object[] { "İşletme Kayıt Ücreti", amount.ToString("F2", System.Globalization.CultureInfo.InvariantCulture), 1 }
                };
                var userBasketJson = JsonSerializer.Serialize(userBasket);

                // Token oluştur - PayTR nokta (.) bekliyor, virgül (,) değil!
                var paymentAmount = amount.ToString("F2", System.Globalization.CultureInfo.InvariantCulture);
                var paymentType = "card";
                var installmentCount = "0";
                var currency = "TL";
                var testMode = _isTestMode ? "1" : "0";
                var non3d = "1"; // Non-3D işlem (kart saklama için)
                
                var hashStr = $"{_merchantId}{userIp}{merchantOid}{email}{paymentAmount}{paymentType}{installmentCount}{currency}{testMode}{non3d}";
                var paytrToken = GenerateToken(hashStr);

                _logger.LogInformation($"PayTR Token generated: {paytrToken.Substring(0, 20)}...");

                // Form data oluştur
                var formData = new Dictionary<string, string>
                {
                    { "merchant_id", _merchantId },
                    { "user_ip", userIp },
                    { "merchant_oid", merchantOid },
                    { "email", email },
                    { "payment_type", paymentType },
                    { "payment_amount", paymentAmount },
                    { "installment_count", installmentCount },
                    { "currency", currency },
                    { "test_mode", testMode },
                    { "non_3d", non3d },
                    { "merchant_ok_url", _merchantOkUrl },
                    { "merchant_fail_url", _merchantFailUrl },
                    { "user_name", userName },
                    { "user_address", userAddress },
                    { "user_phone", userPhone },
                    { "user_basket", userBasketJson },
                    { "debug_on", "1" },
                    { "paytr_token", paytrToken },
                    { "cc_owner", ccOwner },
                    { "card_number", cardNumber },
                    { "expiry_month", expiryMonth },
                    { "expiry_year", expiryYear },
                    { "cvv", cvv },
                    { "store_card", "1" } // Kartı kaydet
                };

                // Eğer mevcut utoken varsa ekle (ikinci kart için)
                if (!string.IsNullOrEmpty(existingUtoken))
                {
                    formData.Add("utoken", existingUtoken);
                    _logger.LogInformation("✅ UToken added to request for grouping cards under same user");
                }

                _logger.LogInformation($"📤 Sending Direct API request to PayTR...");
                _logger.LogInformation($"Store Card: 1, Non-3D: {non3d}");

                // PayTR'ye POST request gönder
                var client = _httpClientFactory.CreateClient();
                var content = new FormUrlEncodedContent(formData);
                var response = await client.PostAsync("https://www.paytr.com/odeme", content);

                var responseContent = await response.Content.ReadAsStringAsync();
                _logger.LogInformation($"📥 PayTR Response: {responseContent}");

                // Non-3D işlem olduğu için direkt sonuç döner
                // Başarılıysa webhook'a bildirim gelecek
                return new PayTRDirectPaymentResponse
                {
                    Success = response.IsSuccessStatusCode,
                    Status = response.IsSuccessStatusCode ? "success" : "failed",
                    MerchantOid = merchantOid,
                    ErrorMessage = response.IsSuccessStatusCode ? null : responseContent
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error in InitiateCardRegistrationPayment");
                return new PayTRDirectPaymentResponse
                {
                    Success = false,
                    Status = "failed",
                    ErrorMessage = ex.Message
                };
            }
        }

        public async Task<PayTRDirectPaymentResponse> ChargeStoredCard(
            string utoken,
            string ctoken,
            string email,
            string userName,
            decimal amount,
            string merchantOid,
            string userIp,
            string? cvv = null)
        {
            try
            {
                _logger.LogInformation($"🔵 Direct API: Charging stored card");
                _logger.LogInformation($"UToken: {utoken.Substring(0, Math.Min(10, utoken.Length))}..., CToken: {ctoken.Substring(0, Math.Min(10, ctoken.Length))}...");
                _logger.LogInformation($"MerchantOid: {merchantOid}, Amount: {amount}");

                // Sepet oluştur
                var userBasket = new[]
                {
                    new object[] { "Aylık Abonelik Ücreti", amount.ToString("F2", System.Globalization.CultureInfo.InvariantCulture), 1 }
                };
                var userBasketJson = JsonSerializer.Serialize(userBasket);

                // Token oluştur - PayTR nokta (.) bekliyor, virgül (,) değil!
                var paymentAmount = amount.ToString("F2", System.Globalization.CultureInfo.InvariantCulture);
                var paymentType = "card";
                var installmentCount = "0";
                var currency = "TL";
                var testMode = _isTestMode ? "1" : "0";
                var non3d = "1"; // Non-3D işlem
                
                var hashStr = $"{_merchantId}{userIp}{merchantOid}{email}{paymentAmount}{paymentType}{installmentCount}{currency}{testMode}{non3d}";
                var paytrToken = GenerateToken(hashStr);

                // Form data oluştur
                var formData = new Dictionary<string, string>
                {
                    { "merchant_id", _merchantId },
                    { "user_ip", userIp },
                    { "merchant_oid", merchantOid },
                    { "email", email },
                    { "payment_type", paymentType },
                    { "payment_amount", paymentAmount },
                    { "installment_count", installmentCount },
                    { "currency", currency },
                    { "test_mode", testMode },
                    { "non_3d", non3d },
                    { "merchant_ok_url", _merchantOkUrl },
                    { "merchant_fail_url", _merchantFailUrl },
                    { "user_name", userName },
                    { "user_address", "Türkiye" }, // Placeholder
                    { "user_phone", "5555555555" }, // Placeholder
                    { "user_basket", userBasketJson },
                    { "debug_on", "1" },
                    { "paytr_token", paytrToken },
                    { "utoken", utoken },
                    { "ctoken", ctoken }
                };

                // CVV varsa ekle
                if (!string.IsNullOrEmpty(cvv))
                {
                    formData.Add("cvv", cvv);
                }

                _logger.LogInformation($"📤 Sending stored card payment request to PayTR...");

                // PayTR'ye POST request gönder
                var client = _httpClientFactory.CreateClient();
                var content = new FormUrlEncodedContent(formData);
                var response = await client.PostAsync("https://www.paytr.com/odeme", content);

                var responseContent = await response.Content.ReadAsStringAsync();
                _logger.LogInformation($"📥 PayTR Response: {responseContent}");

                return new PayTRDirectPaymentResponse
                {
                    Success = response.IsSuccessStatusCode,
                    Status = response.IsSuccessStatusCode ? "success" : "failed",
                    MerchantOid = merchantOid,
                    ErrorMessage = response.IsSuccessStatusCode ? null : responseContent
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error in ChargeStoredCard");
                return new PayTRDirectPaymentResponse
                {
                    Success = false,
                    Status = "failed",
                    ErrorMessage = ex.Message
                };
            }
        }

        public async Task<PayTRCardListResponse> GetStoredCards(string utoken)
        {
            try
            {
                _logger.LogInformation($"🔵 Direct API: Getting stored cards for UToken: {utoken.Substring(0, Math.Min(10, utoken.Length))}...");

                // Token oluştur
                var hashStr = $"{_merchantId}{utoken}";
                var paytrToken = GenerateToken(hashStr);

                var formData = new Dictionary<string, string>
                {
                    { "merchant_id", _merchantId },
                    { "utoken", utoken },
                    { "paytr_token", paytrToken }
                };

                var client = _httpClientFactory.CreateClient();
                var content = new FormUrlEncodedContent(formData);
                var response = await client.PostAsync("https://www.paytr.com/odeme/capi/list", content);

                var responseContent = await response.Content.ReadAsStringAsync();
                _logger.LogInformation($"📥 PayTR Card List Response: {responseContent}");

                // Response parse et
                var json = JsonSerializer.Deserialize<JsonElement>(responseContent);
                var status = json.GetProperty("status").GetString();

                if (status == "success")
                {
                    var cards = new List<PayTRStoredCard>();
                    var cardsArray = json.GetProperty("cards").EnumerateArray();

                    foreach (var card in cardsArray)
                    {
                        cards.Add(new PayTRStoredCard
                        {
                            Ctoken = card.GetProperty("ctoken").GetString(),
                            CardBrand = card.GetProperty("card_brand").GetString(),
                            CardAssociation = card.GetProperty("card_association").GetString(),
                            MaskedPan = card.GetProperty("masked_pan").GetString(),
                            ExpiryMonth = card.GetProperty("expiry_month").GetString(),
                            ExpiryYear = card.GetProperty("expiry_year").GetString(),
                            RequireCvv = card.GetProperty("require_cvv").GetInt32() == 1
                        });
                    }

                    return new PayTRCardListResponse
                    {
                        Success = true,
                        Status = status,
                        Cards = cards
                    };
                }
                else
                {
                    var reason = json.GetProperty("reason").GetString();
                    return new PayTRCardListResponse
                    {
                        Success = false,
                        Status = status,
                        ErrorMessage = reason
                    };
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error in GetStoredCards");
                return new PayTRCardListResponse
                {
                    Success = false,
                    ErrorMessage = ex.Message
                };
            }
        }

        public async Task<bool> DeleteStoredCard(string utoken, string ctoken)
        {
            try
            {
                _logger.LogInformation($"🔵 Direct API: Deleting stored card");
                _logger.LogInformation($"UToken: {utoken.Substring(0, Math.Min(10, utoken.Length))}..., CToken: {ctoken.Substring(0, Math.Min(10, ctoken.Length))}...");

                // Token oluştur
                var hashStr = $"{_merchantId}{utoken}{ctoken}";
                var paytrToken = GenerateToken(hashStr);

                var formData = new Dictionary<string, string>
                {
                    { "merchant_id", _merchantId },
                    { "utoken", utoken },
                    { "ctoken", ctoken },
                    { "paytr_token", paytrToken }
                };

                var client = _httpClientFactory.CreateClient();
                var content = new FormUrlEncodedContent(formData);
                var response = await client.PostAsync("https://www.paytr.com/odeme/capi/delete", content);

                var responseContent = await response.Content.ReadAsStringAsync();
                _logger.LogInformation($"📥 PayTR Delete Card Response: {responseContent}");

                var json = JsonSerializer.Deserialize<JsonElement>(responseContent);
                var status = json.GetProperty("status").GetString();

                return status == "success";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error in DeleteStoredCard");
                return false;
            }
        }

        private string GenerateToken(string hashStr)
        {
            var fullHash = hashStr + _merchantSalt;
            using var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(_merchantKey));
            var hashBytes = hmac.ComputeHash(Encoding.UTF8.GetBytes(fullHash));
            return Convert.ToBase64String(hashBytes);
        }
    }
}
