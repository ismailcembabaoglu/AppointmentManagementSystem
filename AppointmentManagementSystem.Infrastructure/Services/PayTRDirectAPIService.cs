using AppointmentManagementSystem.Application.DTOs.Payment;
using AppointmentManagementSystem.Application.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Globalization;
using System.Linq;
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

                // PayTR dokümantasyonu IPv6 localhost'u kabul etmediği için normalize et
                if (string.IsNullOrWhiteSpace(userIp))
                {
                    userIp = "127.0.0.1";
                }

                if (userIp == "::1" || userIp == "::ffff:127.0.0.1")
                {
                    userIp = "127.0.0.1";
                }
                
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
                var userBasketBase64 = Convert.ToBase64String(Encoding.UTF8.GetBytes(userBasketJson));

                // Token oluştur - PayTR Direct API için
                // PayTR kuruş (integer) format bekler
                var paymentAmount = ((long)Math.Round(amount * 100m, 0, MidpointRounding.AwayFromZero))
                    .ToString(CultureInfo.InvariantCulture);

                var paymentType = "card";
                var installmentCount = "0"; // Taksit yok
                var noInstallment = "1"; // 1 = taksit yapılmayacak, 0 = taksit yapılabilir
                var maxInstallment = "0"; // Maksimum taksit sayısı (0 = taksit yok)
                var currency = "TL";
                var lang = "tr"; // Dil: tr, en, de, fr
                var testMode = _isTestMode ? "1" : "0";
                var non3d = "1"; // Non-3D işlem (kart saklama için ZORUNLU)
                
                // Direct API hash: merchantid + userip + merchantoid + email + paymentamount + paymenttype + installmentcount + currency + testmode + non3d
                // NOT: merchantsalt GenerateToken metodunda ekleniyor
                //var paytrToken = GenerateToken(
                //    _merchantId,
                //    userIp,
                //    merchantOid,
                //    email,
                //    paymentAmount,
                //    paymentType,
                //    installmentCount,
                //    currency,
                //    testMode,
                //    non3d);
                string Birlestir = string.Concat(_merchantId, userIp, merchantOid, email, paymentAmount, paymentType, installmentCount, currency, testMode, non3d, _merchantSalt);
                HMACSHA256 hmac = new HMACSHA256(Encoding.UTF8.GetBytes(_merchantKey));
                byte[] b = hmac.ComputeHash(Encoding.UTF8.GetBytes(Birlestir));
                var paytrToken = Convert.ToBase64String(b);

                // İlk kart kaydında utoken gönderilmezse PayTR kart token üretmiyor
                if (string.IsNullOrWhiteSpace(existingUtoken))
                {
                    var utokenUnique = Guid.NewGuid().ToString("N").Substring(0, 16).ToUpperInvariant();
                    existingUtoken = $"USER_{merchantOid}_{utokenUnique}";
                    _logger.LogInformation($"Generated new UToken for first card: {existingUtoken}");
                }

                _logger.LogInformation($"PayTR Token generated: {paytrToken.Substring(0, 20)}...");

                // Form data oluştur
                // Direct API form data - Kart saklama için
                var formData = new Dictionary<string, string>
                {
                    { "merchant_id", _merchantId },
                    { "user_ip", userIp },
                    { "merchant_oid", merchantOid },
                    { "email", email },
                    { "payment_type", paymentType },
                    { "payment_amount", paymentAmount },
                    { "installment_count", installmentCount }, // ZORUNLU: 0 = taksit yok
                    { "no_installment", noInstallment }, // ZORUNLU: 1 = taksit yapılmayacak
                    { "max_installment", maxInstallment }, // ZORUNLU: 0 = taksit yok
                    { "currency", currency },
                    { "lang", lang }, // ZORUNLU: tr, en, de, fr
                    { "test_mode", testMode },
                    { "non_3d", non3d },
                    { "merchant_ok_url", _merchantOkUrl }, // ZORUNLU
                    { "merchant_fail_url", _merchantFailUrl }, // ZORUNLU
                    { "callback_url", _callbackUrl },
                    { "user_name", userName },
                    { "user_address", userAddress },
                    { "user_phone", userPhone },
                    // PayTR dokümantasyonuna göre basket base64 gönderilmeli
                    { "user_basket", userBasketBase64 },
                    { "debug_on", "1" },
                    { "paytr_token", paytrToken },
                    { "cc_owner", ccOwner },
                    { "card_number", cardNumber },
                    { "expiry_month", expiryMonth },
                    { "expiry_year", expiryYear },
                    { "cvv", cvv },
                    { "store_card", "1" } // Kartı kaydet (ZORUNLU)
                };

                // Kullanıcı token'ını gönder (ilk veya ek kartlar)

                _logger.LogInformation($"✅ UToken added to request: {existingUtoken}");

                _logger.LogInformation($"📤 Sending Direct API request to PayTR...");
                _logger.LogInformation($"Store Card: 1, Non-3D: {non3d}");

                // PayTR'ye POST request gönder
                var client = _httpClientFactory.CreateClient();
                var content = new FormUrlEncodedContent(formData);
                var response = await client.PostAsync("https://www.paytr.com/odeme", content);

                var responseContent = await response.Content.ReadAsStringAsync();
                _logger.LogInformation($"📥 PayTR Response: {responseContent}");

                // Non-3D işlem olduğu için direkt sonuç döner
                // Başarılıysa webhook'a bildirim gelecek ancak kart bilgilerini hemen saklıyoruz
                var maskedPan = MaskCardNumber(cardNumber);

                return new PayTRDirectPaymentResponse
                {
                    Success = response.IsSuccessStatusCode,
                    Status = response.IsSuccessStatusCode ? "success" : "failed",
                    MerchantOid = merchantOid,
                    ErrorMessage = response.IsSuccessStatusCode ? null : responseContent,
                    UserToken = existingUtoken,
                    CardToken = null,
                    MaskedPan = maskedPan,
                    CardBrand = DetectCardBrand(cardNumber)
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

        private string GeneratePayTRToken(string hashString, string merchantSalt, string merchantKey)
        {
            var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(merchantKey));
            var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(hashString));
            return Convert.ToBase64String(hash);
        }

        private static string? DetectCardBrand(string cardNumber)
        {
            var digits = new string((cardNumber ?? string.Empty).Where(char.IsDigit).ToArray());

            if (string.IsNullOrWhiteSpace(digits))
            {
                return null;
            }

            return digits[0] switch
            {
                '4' => "Visa",
                '5' => "Mastercard",
                '3' => "American Express",
                '6' => "Discover",
                _ => "Bilinmiyor"
            };
        }

        private static string? MaskCardNumber(string cardNumber)
        {
            var digits = new string((cardNumber ?? string.Empty).Where(char.IsDigit).ToArray());

            if (digits.Length < 4)
            {
                return null;
            }

            var lastFour = digits[^4..];
            return $"**** **** **** {lastFour}";
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

                if (string.IsNullOrWhiteSpace(userIp))
                {
                    userIp = "127.0.0.1";
                }

                if (userIp == "::1" || userIp == "::ffff:127.0.0.1")
                {
                    userIp = "127.0.0.1";
                }

                // Sepet oluştur
                var userBasket = new[]
                {
                    new object[] { "Aylık Abonelik Ücreti", amount.ToString("F2", System.Globalization.CultureInfo.InvariantCulture), 1 }
                };
                var userBasketJson = JsonSerializer.Serialize(userBasket);
                var userBasketBase64 = Convert.ToBase64String(Encoding.UTF8.GetBytes(userBasketJson));

                // Token oluştur - PayTR Direct API için
                // PayTR Direct API kuruş (integer) bekliyor
                var paymentAmount = FormatAmountToKurus(amount);
                var paymentType = "card";
                var installmentCount = "0"; // Taksit yok
                var noInstallment = "1"; // 1 = taksit yapılmayacak
                var maxInstallment = "0"; // Maksimum taksit sayısı
                var currency = "TL";
                var lang = "tr"; // Dil: tr, en, de, fr
                var testMode = _isTestMode ? "1" : "0";
                var non3d = "1"; // Non-3D işlem
                
                // Direct API hash
                // NOT: merchantsalt GenerateToken metodunda ekleniyor
                var paytrToken = GenerateToken(
                    _merchantId,
                    userIp,
                    merchantOid,
                    email,
                    paymentAmount,
                    paymentType,
                    installmentCount,
                    currency,
                    testMode,
                    non3d);

                // Direct API form data - Kayıtlı karttan ödeme
                var formData = new Dictionary<string, string>
                {
                    { "merchant_id", _merchantId },
                    { "user_ip", userIp },
                    { "merchant_oid", merchantOid },
                    { "email", email },
                    { "payment_type", paymentType },
                    { "payment_amount", paymentAmount },
                    { "installment_count", installmentCount }, // ZORUNLU: 0 = taksit yok
                    { "no_installment", noInstallment }, // ZORUNLU: 1 = taksit yapılmayacak
                    { "max_installment", maxInstallment }, // ZORUNLU: 0 = taksit yok
                    { "currency", currency },
                    { "lang", lang }, // ZORUNLU: tr, en, de, fr
                    { "test_mode", testMode },
                    { "non_3d", non3d },
                    { "merchant_ok_url", _merchantOkUrl }, // ZORUNLU
                    { "merchant_fail_url", _merchantFailUrl }, // ZORUNLU
                    { "callback_url", _callbackUrl },
                    { "user_name", userName },
                    { "user_address", "Türkiye" },
                    { "user_phone", "5555555555" },
                    { "user_basket", userBasketBase64 },
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
                var paytrToken = GenerateToken(_merchantId, utoken);

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
                var paytrToken = GenerateToken(_merchantId, utoken, ctoken);

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

        private string GenerateToken(params string[] parts)
        {
            var hashStr = string.Concat(parts);
            using var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(_merchantKey));
            var hashBytes = hmac.ComputeHash(Encoding.UTF8.GetBytes(hashStr));
            return Convert.ToBase64String(hashBytes);
        }

        private static string FormatAmountToKurus(decimal amount)
        {
            // PayTR miktarı kuruş olarak integer bekliyor. Banka yuvarlama uyumluluğu için ortadan uzak yuvarla.
            var scaled = Math.Round(amount * 100m, 0, MidpointRounding.AwayFromZero);
            return Convert.ToInt64(scaled).ToString(CultureInfo.InvariantCulture);
        }
    }
}
