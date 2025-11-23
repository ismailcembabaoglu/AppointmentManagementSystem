using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using AppointmentManagementSystem.Application.DTOs.Ai;
using AppointmentManagementSystem.Application.Features.Ai.Queries;
using AppointmentManagementSystem.Application.Services;
using AppointmentManagementSystem.Domain.Interfaces;
using MediatR;

namespace AppointmentManagementSystem.Application.Features.Ai.Handlers
{
    public class GetBusinessInsightQueryHandler : IRequestHandler<GetBusinessInsightQuery, AiBusinessInsightResponseDto>
    {
        private readonly IBusinessAnalyticsBuilder _analyticsBuilder;
        private readonly IGeminiClient _geminiClient;

        public GetBusinessInsightQueryHandler(IBusinessAnalyticsBuilder analyticsBuilder, IGeminiClient geminiClient)
        {
            _analyticsBuilder = analyticsBuilder;
            _geminiClient = geminiClient;
        }

        public async Task<AiBusinessInsightResponseDto> Handle(GetBusinessInsightQuery request, CancellationToken cancellationToken)
        {
            var analytics = await _analyticsBuilder.BuildAsync(request.BusinessId, cancellationToken);
            if (analytics == null)
            {
                return new AiBusinessInsightResponseDto
                {
                    Reply = "İşletme bilgileri bulunamadı."
                };
            }

            var prompt = BuildPrompt(analytics, request.Message);
            var reply = await _geminiClient.GenerateContentAsync(prompt, cancellationToken);

            return new AiBusinessInsightResponseDto
            {
                Reply = reply,
                Analytics = analytics
            };
        }

        private static string BuildPrompt(AiBusinessAnalyticsDto analytics, string message)
        {
            var builder = new StringBuilder();
            builder.AppendLine("Sen deneyimli bir işletme danışmanısın. Kısa, aksiyon odaklı yanıt ver.");
            builder.AppendLine($"İşletme: {analytics.BusinessName} ({analytics.CategoryName})");
            builder.AppendLine($"Lokasyon: {analytics.City} / {analytics.District}");
            builder.AppendLine($"Toplam randevu: {analytics.Stats.TotalAppointments}, tamamlanan: {analytics.Stats.CompletedAppointments}, bekleyen: {analytics.Stats.PendingAppointments}, iptal: {analytics.Stats.CancelledAppointments}");
            builder.AppendLine($"Ortalama puan: {(analytics.Stats.AverageRating.HasValue ? analytics.Stats.AverageRating.Value.ToString("0.0") : "Veri yok")}, tamamlananlardan tahmini gelir: {analytics.Stats.TotalRevenue:0.00} TL");

            builder.AppendLine("Hizmet performansları (ilk 5):");
            foreach (var service in analytics.Services.Take(5))
            {
                builder.AppendLine($"- {service.Name}: {service.CompletedCount} tamamlanan, gelir {service.Revenue:0.00} TL, ortalama puan {(service.AverageRating.HasValue ? service.AverageRating.Value.ToString("0.0") : "-"})");
            }

            builder.AppendLine("Son 5 yorum:");
            foreach (var review in analytics.RecentReviews.Take(5))
            {
                builder.AppendLine($"- {review.CustomerName}: {review.Rating}/5 " + (string.IsNullOrWhiteSpace(review.Review) ? string.Empty : review.Review));
            }

            builder.AppendLine("Talep edilen konu:");
            builder.AppendLine(string.IsNullOrWhiteSpace(message) ? "Genel durum değerlendirmesi yap ve 3 aksiyon öner." : message);
            builder.AppendLine("Çıktı formatı: En fazla 4 madde, ölçülebilir aksiyonlar, gerekiyorsa hızlı KPI listesi, iletişim dilini samimi tut.");

            return builder.ToString();
        }
    }
}
