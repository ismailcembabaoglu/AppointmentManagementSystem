using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AppointmentManagementSystem.Application.DTOs.Ai;
using AppointmentManagementSystem.Application.Features.Ai.Queries;
using AppointmentManagementSystem.Application.Services;
using ClosedXML.Excel;
using MediatR;

namespace AppointmentManagementSystem.Application.Features.Ai.Handlers
{
    public class GetBusinessReportFileQueryHandler : IRequestHandler<GetBusinessReportFileQuery, AiBusinessReportFileDto?>
    {
        private readonly IBusinessAnalyticsBuilder _analyticsBuilder;

        public GetBusinessReportFileQueryHandler(IBusinessAnalyticsBuilder analyticsBuilder)
        {
            _analyticsBuilder = analyticsBuilder;
        }

        public async Task<AiBusinessReportFileDto?> Handle(GetBusinessReportFileQuery request, CancellationToken cancellationToken)
        {
            var analytics = await _analyticsBuilder.BuildAsync(request.BusinessId, cancellationToken);
            if (analytics == null)
            {
                return null;
            }

            using var workbook = new XLWorkbook();
            BuildSummarySheet(workbook, analytics);
            BuildServiceSheet(workbook, analytics);
            BuildAppointmentSheet(workbook, analytics);
            BuildReviewSheet(workbook, analytics);

            using var stream = new MemoryStream();
            workbook.SaveAs(stream);

            return new AiBusinessReportFileDto
            {
                FileName = $"{analytics.BusinessName}-rapor-{DateTime.UtcNow:yyyyMMdd}.xlsx",
                Base64Data = Convert.ToBase64String(stream.ToArray())
            };
        }

        private static void BuildSummarySheet(IXLWorkbook workbook, AiBusinessAnalyticsDto analytics)
        {
            var sheet = workbook.AddWorksheet("Özet");
            sheet.Cell(1, 1).Value = "İşletme";
            sheet.Cell(1, 2).Value = analytics.BusinessName;
            sheet.Cell(2, 1).Value = "Kategori";
            sheet.Cell(2, 2).Value = analytics.CategoryName;
            sheet.Cell(3, 1).Value = "Lokasyon";
            sheet.Cell(3, 2).Value = $"{analytics.City} / {analytics.District}";

            sheet.Cell(5, 1).Value = "Toplam Randevu";
            sheet.Cell(5, 2).Value = analytics.Stats.TotalAppointments;
            sheet.Cell(6, 1).Value = "Tamamlanan";
            sheet.Cell(6, 2).Value = analytics.Stats.CompletedAppointments;
            sheet.Cell(7, 1).Value = "Bekleyen";
            sheet.Cell(7, 2).Value = analytics.Stats.PendingAppointments;
            sheet.Cell(8, 1).Value = "İptal";
            sheet.Cell(8, 2).Value = analytics.Stats.CancelledAppointments;
            sheet.Cell(9, 1).Value = "Ortalama Puan";
            sheet.Cell(9, 2).Value = analytics.Stats.AverageRating;
            sheet.Cell(10, 1).Value = "Tamamlanan Geliri";
            sheet.Cell(10, 2).Value = analytics.Stats.TotalRevenue;

            sheet.Columns().AdjustToContents();
        }

        private static void BuildServiceSheet(IXLWorkbook workbook, AiBusinessAnalyticsDto analytics)
        {
            var sheet = workbook.AddWorksheet("Hizmetler");
            sheet.Cell(1, 1).Value = "Hizmet";
            sheet.Cell(1, 2).Value = "Fiyat";
            sheet.Cell(1, 3).Value = "Randevu";
            sheet.Cell(1, 4).Value = "Tamamlanan";
            sheet.Cell(1, 5).Value = "Gelir";
            sheet.Cell(1, 6).Value = "Ortalama Puan";

            var row = 2;
            foreach (var service in analytics.Services)
            {
                sheet.Cell(row, 1).Value = service.Name;
                sheet.Cell(row, 2).Value = service.Price;
                sheet.Cell(row, 3).Value = service.BookingCount;
                sheet.Cell(row, 4).Value = service.CompletedCount;
                sheet.Cell(row, 5).Value = service.Revenue;
                sheet.Cell(row, 6).Value = service.AverageRating;
                row++;
            }

            sheet.Columns().AdjustToContents();
        }

        private static void BuildAppointmentSheet(IXLWorkbook workbook, AiBusinessAnalyticsDto analytics)
        {
            var sheet = workbook.AddWorksheet("Randevular");
            sheet.Cell(1, 1).Value = "Tarih";
            sheet.Cell(1, 2).Value = "Hizmet";
            sheet.Cell(1, 3).Value = "Müşteri";
            sheet.Cell(1, 4).Value = "Personel";
            sheet.Cell(1, 5).Value = "Durum";
            sheet.Cell(1, 6).Value = "Fiyat";
            sheet.Cell(1, 7).Value = "Puan";

            var row = 2;
            foreach (var appointment in analytics.RecentAppointments.OrderByDescending(a => a.AppointmentDate))
            {
                sheet.Cell(row, 1).Value = appointment.AppointmentDate;
                sheet.Cell(row, 2).Value = appointment.ServiceName;
                sheet.Cell(row, 3).Value = appointment.CustomerName;
                sheet.Cell(row, 4).Value = appointment.EmployeeName;
                sheet.Cell(row, 5).Value = appointment.Status;
                sheet.Cell(row, 6).Value = appointment.Price;
                sheet.Cell(row, 7).Value = appointment.Rating;
                row++;
            }

            sheet.Columns().AdjustToContents();
        }

        private static void BuildReviewSheet(IXLWorkbook workbook, AiBusinessAnalyticsDto analytics)
        {
            var sheet = workbook.AddWorksheet("Yorumlar");
            sheet.Cell(1, 1).Value = "Müşteri";
            sheet.Cell(1, 2).Value = "Hizmet";
            sheet.Cell(1, 3).Value = "Puan";
            sheet.Cell(1, 4).Value = "Yorum";
            sheet.Cell(1, 5).Value = "Tarih";

            var row = 2;
            foreach (var review in analytics.RecentReviews)
            {
                sheet.Cell(row, 1).Value = review.CustomerName;
                sheet.Cell(row, 2).Value = review.ServiceName;
                sheet.Cell(row, 3).Value = review.Rating;
                sheet.Cell(row, 4).Value = review.Review;
                sheet.Cell(row, 5).Value = review.RatingDate;
                row++;
            }

            sheet.Columns().AdjustToContents();
        }
    }
}
