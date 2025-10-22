using AppointmentManagementSystem.API.Controllers;
using AppointmentManagementSystem.Application.DTOs;
using AppointmentManagementSystem.Application.Features.Services.Commands;
using AppointmentManagementSystem.Application.Features.Services.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AppointmentManagementSystem.API.Controllers
{
    public class ServicesController : BaseController
    {
        private readonly IMediator _mediator;

        public ServicesController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] int? businessId = null)
        {
            var query = new GetAllServicesQuery { BusinessId = businessId };
            var result = await _mediator.Send(query);
            return OkResponse(result, "Hizmetler başarıyla getirildi.");
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var query = new GetServiceByIdQuery { Id = id };
            var result = await _mediator.Send(query);

            if (result == null)
                return ErrorResponse<ServiceDto>("Hizmet bulunamadı.", new List<string> { "Geçersiz hizmet ID" });

            return OkResponse(result, "Hizmet başarıyla getirildi.");
        }

        [HttpPost]
        [Authorize(Roles = "Business")]
        public async Task<IActionResult> Create([FromBody] CreateServiceDto createServiceDto)
        {
            var command = new CreateServiceCommand { CreateServiceDto = createServiceDto };
            var result = await _mediator.Send(command);
            return OkResponse(result, "Hizmet başarıyla oluşturuldu.");
        }

        [HttpPut("{id:int}")]
        [Authorize(Roles = "Business")]
        public async Task<IActionResult> Update(int id, [FromBody] CreateServiceDto updateServiceDto)
        {
            var command = new UpdateServiceCommand
            {
                Id = id,
                CreateServiceDto = updateServiceDto
            };
            var result = await _mediator.Send(command);

            if (result == null)
                return ErrorResponse<ServiceDto>("Hizmet bulunamadı.", new List<string> { "Geçersiz hizmet ID" });

            return OkResponse(result, "Hizmet başarıyla güncellendi.");
        }

        [HttpDelete("{id:int}")]
        [Authorize(Roles = "Business,Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var command = new DeleteServiceCommand { Id = id };
            var result = await _mediator.Send(command);

            if (!result)
                return ErrorResponse<bool>("Hizmet bulunamadı.", new List<string> { "Geçersiz hizmet ID" });

            return OkResponse(result, "Hizmet başarıyla silindi.");
        }
    }
}
