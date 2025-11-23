using System.Collections.Generic;
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
    }
}
