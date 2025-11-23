using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AppointmentManagementSystem.Application.DTOs.Ai;
using AppointmentManagementSystem.Application.Features.Ai.Queries;
using AppointmentManagementSystem.Domain.Interfaces;
using MediatR;

namespace AppointmentManagementSystem.Application.Features.Ai.Handlers
{
    public class GetAiRecommendationsQueryHandler : IRequestHandler<GetAiRecommendationsQuery, AiRecommendationResponseDto>
    {
        private readonly IBusinessRepository _businessRepository;
        private readonly IAppointmentRepository _appointmentRepository;
        private readonly IGeminiClient _geminiClient;

        public GetAiRecommendationsQueryHandler(
            IBusinessRepository businessRepository,
            IAppointmentRepository appointmentRepository,
            IGeminiClient geminiClient)
        {
            _businessRepository = businessRepository;
            _appointmentRepository = appointmentRepository;
            _geminiClient = geminiClient;
        }

        public async Task<AiRecommendationResponseDto> Handle(GetAiRecommendationsQuery request, CancellationToken cancellationToken)
        {
            var maxResults = Math.Clamp(request.MaxResults <= 0 ? 3 : request.MaxResults, 1, 5);

            var businesses = await _businessRepository.GetAllWithDetailsAsync(
                request.CategoryId,
                null,
                request.City,
                request.District);

            if (!businesses.Any())
            {
                return new AiRecommendationResponseDto
                {
                    Reply = "Belirttiğiniz kriterlerde aktif bir işletme bulunamadı. Farklı bir ilçe, il veya kategori ile tekrar deneyebilirsiniz."
                };
            }

            var enriched = new List<AiBusinessRecommendationDto>();
            foreach (var business in businesses)
            {
                var appointments = await _appointmentRepository.GetAllWithDetailsAsync(businessId: business.Id);
                var ratedAppointments = appointments.Where(a => a.Rating.HasValue && a.Status == "Completed").ToList();

                var ratings = ratedAppointments.Select(a => a.Rating!.Value).ToList();
                var highlightedReviews = ratedAppointments
                    .Where(a => !string.IsNullOrWhiteSpace(a.Review))
                    .OrderByDescending(a => a.RatingDate ?? a.UpdatedAt ?? a.AppointmentDate)
                    .Take(3)
                    .Select(a => $"{(a.Customer?.Name ?? "Anonim").Trim()}: {a.Review!.Trim()}")
                    .ToList();

                var average = ratings.Any() ? Math.Round(ratings.Average(), 2) : 0;

                enriched.Add(new AiBusinessRecommendationDto
                {
                    BusinessId = business.Id,
                    Name = business.Name,
                    Category = business.Category?.Name ?? string.Empty,
                    City = business.City,
                    District = business.District,
                    AverageRating = average,
                    TotalRatings = ratings.Count,
                    HighlightedReviews = highlightedReviews
                });
            }

            var ranked = enriched
                .OrderByDescending(b => b.AverageRating)
                .ThenByDescending(b => b.TotalRatings)
                .ThenBy(b => b.Name)
                .Take(maxResults)
                .ToList();

            var prompt = BuildPrompt(request, ranked, businesses.Count());
            var reply = await _geminiClient.GenerateContentAsync(prompt, cancellationToken);

            return new AiRecommendationResponseDto
            {
                Reply = reply,
                Recommendations = ranked
            };
        }

        private static string BuildPrompt(GetAiRecommendationsQuery request, IReadOnlyCollection<AiBusinessRecommendationDto> businesses, int totalFound)
        {
            var builder = new StringBuilder();
            builder.AppendLine("Sen aptivaplan.com.tr kullanıcılarına rehberlik eden bir yapay zeka asistanısın.");
            builder.AppendLine("Cevabını Türkçe ve samimi bir tonda yaz.");
            builder.AppendLine("Sadece listelenen işletmeleri kullan ve yeni işletme uydurma.");
            builder.AppendLine();
            builder.AppendLine($"Kullanıcı isteği: {request.Message}");

            if (!string.IsNullOrWhiteSpace(request.City))
            {
                builder.AppendLine($"Şehir: {request.City}");
            }

            if (!string.IsNullOrWhiteSpace(request.District))
            {
                builder.AppendLine($"İlçe: {request.District}");
            }

            if (request.CategoryId.HasValue)
            {
                builder.AppendLine($"Kategori Id: {request.CategoryId}");
            }

            builder.AppendLine();
            builder.AppendLine($"Toplam {totalFound} uygun işletme bulundu. Öne çıkarılacak {businesses.Count} işletme:");

            var index = 1;
            foreach (var business in businesses)
            {
                builder.AppendLine($"{index}. {business.Name} (Id: {business.BusinessId}) - {business.Category} - {business.City}/{business.District}");
                builder.AppendLine($"   Ortalama puan: {business.AverageRating:0.0} ({business.TotalRatings} değerlendirme)");

                if (business.HighlightedReviews.Any())
                {
                    builder.AppendLine("   Öne çıkan yorumlar:");
                    foreach (var review in business.HighlightedReviews)
                    {
                        builder.AppendLine($"   - {review}");
                    }
                }

                builder.AppendLine();
                index++;
            }

            builder.AppendLine("Cevap formatı:");
            builder.AppendLine("- Kısa bir giriş cümlesi yaz.");
            builder.AppendLine("- Ardından madde işaretli listede her işletme için: isim, ortalama puan, yorum sayısı ve güçlü yanlarını belirt.");
            builder.AppendLine("- Her maddeye https://aptivaplan.com.tr/business/{id} veya /business/{id} şeklinde yönlendirme ekle.");
            builder.AppendLine("- Kullanıcının talebine göre ekstra öneri veya ipucu verebilirsin.");

            return builder.ToString();
        }
    }
}
