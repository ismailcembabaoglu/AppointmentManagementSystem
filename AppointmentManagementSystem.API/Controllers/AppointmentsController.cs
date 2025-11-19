using AppointmentManagementSystem.API.Controllers;
using AppointmentManagementSystem.API.Services;
using AppointmentManagementSystem.Application.DTOs;
using AppointmentManagementSystem.Application.Features.Appointments.Commands;
using AppointmentManagementSystem.Application.Features.Appointments.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AppointmentManagementSystem.API.Controllers
{
    public class AppointmentsController : BaseController
    {
        private readonly IMediator _mediator;
        private readonly INotificationService _notificationService;

        public AppointmentsController(IMediator mediator, INotificationService notificationService)
        {
            _mediator = mediator;
            _notificationService = notificationService;
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetAll([FromQuery] int? customerId = null, [FromQuery] int? businessId = null)
        {
            var query = new GetAllAppointmentsQuery
            {
                CustomerId = customerId,
                BusinessId = businessId
            };
            var result = await _mediator.Send(query);
            return OkResponse(result, "Randevular başarıyla getirildi.");
        }

        [HttpGet("{id:int}")]
        [Authorize]
        public async Task<IActionResult> GetById(int id)
        {
            var query = new GetAppointmentByIdQuery { Id = id };
            var result = await _mediator.Send(query);

            if (result == null)
                return ErrorResponse<AppointmentDto>("Randevu bulunamadı.", new List<string> { "Geçersiz randevu ID" });

            return OkResponse(result, "Randevu başarıyla getirildi.");
        }

        [HttpPost]
        [Authorize(Roles = "Customer,Business")]
        public async Task<IActionResult> Create([FromBody] CreateAppointmentDto createAppointmentDto)
        {
            var command = new CreateAppointmentCommand { CreateAppointmentDto = createAppointmentDto };
            var result = await _mediator.Send(command);
            
            // SignalR ile bildirim gönder
            if (result != null)
            {
                await _notificationService.NotifyAppointmentCreated(result, result.CustomerId, result.BusinessId);
            }
            
            return OkResponse(result, "Randevu başarıyla oluşturuldu.");
        }

        [HttpPut("{id:int}/status")]
        [Authorize(Roles = "Business,Customer")]
        public async Task<IActionResult> UpdateStatus(int id, [FromBody] UpdateAppointmentStatusDto updateStatusDto)
        {
            var command = new UpdateAppointmentStatusCommand
            {
                Id = id,
                Status = updateStatusDto.Status
            };
            var result = await _mediator.Send(command);

            if (result == null)
                return ErrorResponse<AppointmentDto>("Randevu bulunamadı.", new List<string> { "Geçersiz randevu ID" });

            // SignalR ile durum değişikliği bildirimi gönder
            await _notificationService.NotifyAppointmentStatusChanged(result, result.CustomerId, result.BusinessId);

            return OkResponse(result, "Randevu durumu başarıyla güncellendi.");
        }

        [HttpPut("{id:int}/rating")]
        [Authorize(Roles = "Customer")]
        public async Task<IActionResult> AddRating(int id, [FromBody] AddAppointmentRatingDto ratingDto)
        {
            var command = new AddAppointmentRatingCommand
            {
                Id = id,
                Rating = ratingDto.Rating,
                Review = ratingDto.Review
            };
            var result = await _mediator.Send(command);

            if (result == null)
                return ErrorResponse<AppointmentDto>("Randevu bulunamadı.", new List<string> { "Geçersiz randevu ID" });

            return OkResponse(result, "Değerlendirme başarıyla eklendi.");
        }

        [HttpDelete("{id:int}")]
        [Authorize(Roles = "Customer,Business,Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var command = new DeleteAppointmentCommand { Id = id };
            var result = await _mediator.Send(command);

            if (!result)
                return ErrorResponse<bool>("Randevu bulunamadı.", new List<string> { "Geçersiz randevu ID" });

            return OkResponse(result, "Randevu başarıyla silindi.");
        }
    }
}
