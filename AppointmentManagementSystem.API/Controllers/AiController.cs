using System.Collections.Generic;
using System.Linq;
using AppointmentManagementSystem.Application.DTOs.Ai;
using AppointmentManagementSystem.Application.Features.Ai.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AppointmentManagementSystem.API.Controllers
{
    public class AiController : BaseController
    {
        private readonly IMediator _mediator;

        public AiController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("recommendations")]
        [Authorize(Roles = "Customer")]
        public async Task<IActionResult> GetRecommendations([FromBody] AiRecommendationRequestDto requestDto)
        {
            if (string.IsNullOrWhiteSpace(requestDto.Message))
            {
                return ErrorResponse<AiRecommendationResponseDto>(
                    "Lütfen işletme talebinizi kısaca yazın.",
                    new List<string> { "Mesaj gereklidir." });
            }

            var query = new GetAiRecommendationsQuery
            {
                Message = requestDto.Message,
                City = requestDto.City,
                District = requestDto.District,
                CategoryId = requestDto.CategoryId,
                MaxResults = requestDto.MaxResults
            };

            var result = await _mediator.Send(query);
            return OkResponse(result, "Yapay zeka önerileri hazır.");
        }

        [HttpPost("business/insights")]
        [Authorize(Roles = "Business")]
        public async Task<IActionResult> GetBusinessInsights([FromBody] AiBusinessInsightRequestDto requestDto)
        {
            var businessIdClaim = User.Claims.FirstOrDefault(c => c.Type == "businessId");
            if (businessIdClaim == null || !int.TryParse(businessIdClaim.Value, out var businessId))
            {
                return ErrorResponse<AiBusinessInsightResponseDto>("İşletme bilgisi bulunamadı.");
            }

            var query = new GetBusinessInsightQuery
            {
                BusinessId = businessId,
                Message = requestDto.Message
            };

            var result = await _mediator.Send(query);
            return OkResponse(result, "İşletme analizi hazır.");
        }

        [HttpGet("business/report")]
        [Authorize(Roles = "Business")]
        public async Task<IActionResult> DownloadBusinessReport()
        {
            var businessIdClaim = User.Claims.FirstOrDefault(c => c.Type == "businessId");
            if (businessIdClaim == null || !int.TryParse(businessIdClaim.Value, out var businessId))
            {
                return ErrorResponse<AiBusinessReportFileDto>("İşletme bilgisi bulunamadı.");
            }

            var result = await _mediator.Send(new GetBusinessReportFileQuery { BusinessId = businessId });
            if (result == null)
            {
                return ErrorResponse<AiBusinessReportFileDto>("Rapor oluşturulamadı.");
            }

            return OkResponse(result, "Rapor hazır.");
        }
    }
}
