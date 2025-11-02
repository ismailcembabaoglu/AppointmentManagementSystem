using AppointmentManagementSystem.API.Controllers;
using AppointmentManagementSystem.Application.DTOs;
using AppointmentManagementSystem.Application.Features.Employees.Commands;
using AppointmentManagementSystem.Application.Features.Employees.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AppointmentManagementSystem.API.Controllers
{
    public class EmployeesController : BaseController
    {
        private readonly IMediator _mediator;

        public EmployeesController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetAll([FromQuery] int? businessId = null)
        {
            var query = new GetAllEmployeesQuery { BusinessId = businessId };
            var result = await _mediator.Send(query);
            return OkResponse(result, "Çalışanlar başarıyla getirildi.");
        }

        [HttpGet("available")]
        [AllowAnonymous]
        public async Task<IActionResult> GetAvailableEmployees(
            [FromQuery] int businessId,
            [FromQuery] DateTime selectedDate,
            [FromQuery] TimeSpan selectedTime,
            [FromQuery] int totalDurationMinutes)
        {
            var query = new GetAvailableEmployeesQuery
            {
                BusinessId = businessId,
                SelectedDate = selectedDate,
                SelectedTime = selectedTime,
                TotalDurationMinutes = totalDurationMinutes
            };
            var result = await _mediator.Send(query);
            return OkResponse(result, "Müsait çalışanlar başarıyla getirildi.");
        }

        [HttpGet("{id:int}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetById(int id)
        {
            var query = new GetEmployeeByIdQuery { Id = id };
            var result = await _mediator.Send(query);

            if (result == null)
                return ErrorResponse<EmployeeDto>("Çalışan bulunamadı.", new List<string> { "Geçersiz çalışan ID" });

            return OkResponse(result, "Çalışan başarıyla getirildi.");
        }

        [HttpPost]
        [Authorize(Roles = "Business")]
        public async Task<IActionResult> Create([FromBody] CreateEmployeeDto createEmployeeDto)
        {
            var command = new CreateEmployeeCommand { CreateEmployeeDto = createEmployeeDto };
            var result = await _mediator.Send(command);
            return OkResponse(result, "Çalışan başarıyla oluşturuldu.");
        }

        [HttpPut("{id:int}")]
        [Authorize(Roles = "Business")]
        public async Task<IActionResult> Update(int id, [FromBody] CreateEmployeeDto updateEmployeeDto)
        {
            var command = new UpdateEmployeeCommand
            {
                Id = id,
                CreateEmployeeDto = updateEmployeeDto
            };
            var result = await _mediator.Send(command);

            if (result == null)
                return ErrorResponse<EmployeeDto>("Çalışan bulunamadı.", new List<string> { "Geçersiz çalışan ID" });

            return OkResponse(result, "Çalışan başarıyla güncellendi.");
        }

        [HttpDelete("{id:int}")]
        [Authorize(Roles = "Business,Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var command = new DeleteEmployeeCommand { Id = id };
            var result = await _mediator.Send(command);

            if (!result)
                return ErrorResponse<bool>("Çalışan bulunamadı.", new List<string> { "Geçersiz çalışan ID" });

            return OkResponse(result, "Çalışan başarıyla silindi.");
        }
    }
}
