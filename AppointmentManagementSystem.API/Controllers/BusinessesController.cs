using AppointmentManagementSystem.API.Controllers;
using AppointmentManagementSystem.Application.DTOs;
using AppointmentManagementSystem.Application.Features.Businesses.Commands;
using AppointmentManagementSystem.Application.Features.Businesses.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AppointmentManagementSystem.API.Controllers
{
    public class BusinessesController : BaseController
    {
        private readonly IMediator _mediator;

        public BusinessesController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetAll([FromQuery] int? categoryId = null, [FromQuery] string? search = null)
        {
            var query = new GetAllBusinessesQuery
            {
                CategoryId = categoryId,
                SearchTerm = search
            };
            var result = await _mediator.Send(query);
            return OkResponse(result, "İşletmeler başarıyla getirildi.");
        }

        [HttpGet("{id:int}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetById(int id)
        {
            var query = new GetBusinessByIdQuery { Id = id };
            var result = await _mediator.Send(query);

            if (result == null)
                return ErrorResponse<BusinessDto>("İşletme bulunamadı.", new List<string> { "Geçersiz işletme ID" });

            return OkResponse(result, "İşletme başarıyla getirildi.");
        }

        [HttpGet("{id:int}/services")]
        [AllowAnonymous]
        public async Task<IActionResult> GetServices(int id)
        {
            var query = new GetServicesByBusinessQuery { BusinessId = id };
            var result = await _mediator.Send(query);
            return OkResponse(result, "İşletmeye ait hizmetler başarıyla getirildi.");
        }

        [HttpGet("{id:int}/employees")]
        [AllowAnonymous]
        public async Task<IActionResult> GetEmployees(int id)
        {
            var query = new GetEmployeesByBusinessQuery { BusinessId = id };
            var result = await _mediator.Send(query);
            return OkResponse(result, "İşletmeye ait çalışanlar başarıyla getirildi.");
        }

        [HttpGet("{id:int}/appointments")]
        [Authorize(Roles = "Business,Admin")]
        public async Task<IActionResult> GetAppointments(int id)
        {
            var query = new GetAppointmentsByBusinessQuery { BusinessId = id };
            var result = await _mediator.Send(query);
            return OkResponse(result, "İşletmeye ait randevular başarıyla getirildi.");
        }

        [HttpPost]
        [Authorize(Roles = "Business")]
        public async Task<IActionResult> Create([FromBody] CreateBusinessDto createBusinessDto)
        {
            var command = new CreateBusinessCommand { CreateBusinessDto = createBusinessDto };
            var result = await _mediator.Send(command);
            return OkResponse(result, "İşletme başarıyla oluşturuldu.");
        }

        [HttpPut("{id:int}")]
        [Authorize(Roles = "Business")]
        public async Task<IActionResult> Update(int id, [FromBody] CreateBusinessDto updateBusinessDto)
        {
            var command = new UpdateBusinessCommand
            {
                Id = id,
                CreateBusinessDto = updateBusinessDto
            };
            var result = await _mediator.Send(command);

            if (result == null)
                return ErrorResponse<BusinessDto>("İşletme bulunamadı.", new List<string> { "Geçersiz işletme ID" });

            return OkResponse(result, "İşletme başarıyla güncellendi.");
        }

        [HttpDelete("{id:int}")]
        [Authorize(Roles = "Business,Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var command = new DeleteBusinessCommand { Id = id };
            var result = await _mediator.Send(command);

            if (!result)
                return ErrorResponse<bool>("İşletme bulunamadı.", new List<string> { "Geçersiz işletme ID" });

            return OkResponse(result, "İşletme başarıyla silindi.");
        }
    }
}
