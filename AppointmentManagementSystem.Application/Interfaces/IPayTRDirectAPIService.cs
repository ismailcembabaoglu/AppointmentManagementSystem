using AppointmentManagementSystem.Application.DTOs.Payment;

namespace AppointmentManagementSystem.Application.Interfaces
{
    /// <summary>
    /// PayTR Direct API Service Interface
    /// Kart saklama ve recurring payment için Direct API kullanır
    /// </summary>
    public interface IPayTRDirectAPIService
    {
        /// <summary>
        /// İlk kayıt sırasında yeni kart kaydeder ve ödeme alır
        /// </summary>
        Task<PayTRDirectPaymentResponse> InitiateCardRegistrationPayment(
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
            string? existingUtoken = null); // İkinci kart eklerken mevcut utoken gönderilir

        /// <summary>
        /// Kayıtlı karttan ödeme alır (Recurring payment için)
        /// </summary>
        Task<PayTRDirectPaymentResponse> ChargeStoredCard(
            string utoken,
            string ctoken,
            string email,
            string userName,
            decimal amount,
            string merchantOid,
            string userIp,
            string? cvv = null); // CVV bazen gerekli olabilir

        /// <summary>
        /// Kullanıcının kayıtlı kart listesini getirir
        /// </summary>
        Task<PayTRCardListResponse> GetStoredCards(string utoken);

        /// <summary>
        /// Kayıtlı kartı siler
        /// </summary>
        Task<bool> DeleteStoredCard(string utoken, string ctoken);
    }
}
