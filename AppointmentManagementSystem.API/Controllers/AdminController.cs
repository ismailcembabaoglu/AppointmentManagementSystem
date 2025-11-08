using AppointmentManagementSystem.Application.Features.Admin.Commands;
using AppointmentManagementSystem.Application.Features.Admin.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AppointmentManagementSystem.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class AdminController : ControllerBase
    {
        private readonly IMediator _mediator;

        public AdminController(IMediator mediator)
        {
            _mediator = mediator;
        }

        // Dashboard Stats
        [HttpGet("dashboard/stats")]
        public async Task<IActionResult> GetDashboardStats()
        {
            var result = await _mediator.Send(new GetAdminDashboardStatsQuery());
            return Ok(result);
        }

        // Business Management
        [HttpGet("businesses")]
        public async Task<IActionResult> GetAllBusinesses(
            [FromQuery] string? searchTerm,
            [FromQuery] bool? isActive,
            [FromQuery] int? categoryId)
        {
            var query = new GetAllBusinessesAdminQuery
            {
                SearchTerm = searchTerm,
                IsActive = isActive,
                CategoryId = categoryId
            };
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        [HttpGet("businesses/{businessId}")]
        public async Task<IActionResult> GetBusinessDetail(int businessId)
        {
            var query = new GetBusinessDetailAdminQuery { BusinessId = businessId };
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        [HttpPut("businesses/{businessId}/status")]
        public async Task<IActionResult> UpdateBusinessStatus(int businessId, [FromBody] UpdateBusinessStatusRequest request)
        {
            var command = new UpdateBusinessStatusCommand
            {
                BusinessId = businessId,
                IsActive = request.IsActive
            };
            var result = await _mediator.Send(command);
            return result ? Ok(new { message = "Business status updated successfully" }) : BadRequest(new { message = "Failed to update business status" });
        }

        // Appointments Management
        [HttpGet("businesses/{businessId}/appointments")]
        public async Task<IActionResult> GetBusinessAppointments(
            int businessId,
            [FromQuery] DateTime? startDate,
            [FromQuery] DateTime? endDate,
            [FromQuery] string? status)
        {
            var query = new GetBusinessAppointmentsAdminQuery
            {
                BusinessId = businessId,
                StartDate = startDate,
                EndDate = endDate,
                Status = status
            };
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        [HttpDelete("appointments/{appointmentId}")]
        public async Task<IActionResult> DeleteAppointment(int appointmentId)
        {
            var command = new DeleteAppointmentAdminCommand { AppointmentId = appointmentId };
            var result = await _mediator.Send(command);
            return result ? Ok(new { message = "Appointment deleted successfully" }) : BadRequest(new { message = "Failed to delete appointment" });
        }

        [HttpPut("appointments/{appointmentId}/status")]
        public async Task<IActionResult> UpdateAppointmentStatus(int appointmentId, [FromBody] UpdateAppointmentStatusRequest request)
        {
            var command = new UpdateAppointmentStatusAdminCommand
            {
                AppointmentId = appointmentId,
                Status = request.Status
            };
            var result = await _mediator.Send(command);
            return result ? Ok(new { message = "Appointment status updated successfully" }) : BadRequest(new { message = "Failed to update appointment status" });
        }

        // Employee Management
        [HttpDelete("employees/{employeeId}")]
        public async Task<IActionResult> DeleteEmployee(int employeeId)
        {
            var command = new DeleteEmployeeAdminCommand { EmployeeId = employeeId };
            var result = await _mediator.Send(command);
            return result ? Ok(new { message = "Employee deleted successfully" }) : BadRequest(new { message = "Failed to delete employee" });
        }

        // Payment Management
        [HttpGet("businesses/{businessId}/payments")]
        public async Task<IActionResult> GetBusinessPayments(
            int businessId,
            [FromQuery] DateTime? startDate,
            [FromQuery] DateTime? endDate)
        {
            var query = new GetBusinessPaymentsAdminQuery
            {
                BusinessId = businessId,
                StartDate = startDate,
                EndDate = endDate
            };
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        [HttpPut("businesses/{businessId}/subscription/auto-renewal")]
        public async Task<IActionResult> UpdateSubscriptionAutoRenewal(int businessId, [FromBody] UpdateAutoRenewalRequest request)
        {
            var command = new UpdateSubscriptionAutoRenewalCommand
            {
                BusinessId = businessId,
                AutoRenewal = request.AutoRenewal
            };
            var result = await _mediator.Send(command);
            return result ? Ok(new { message = "Auto-renewal updated successfully" }) : BadRequest(new { message = "Failed to update auto-renewal" });
        }

        [HttpPost("payments/{paymentId}/refund")]
        public async Task<IActionResult> RefundPayment(int paymentId, [FromBody] RefundPaymentRequest request)
        {
            var command = new RefundPaymentCommand
            {
                PaymentId = paymentId,
                Reason = request.Reason
            };
            var result = await _mediator.Send(command);
            return result ? Ok(new { message = "Payment refunded successfully" }) : BadRequest(new { message = "Failed to refund payment" });
        }

        // Reports
        [HttpGet("reports")]
        public async Task<IActionResult> GetReports(
            [FromQuery] DateTime? startDate,
            [FromQuery] DateTime? endDate)
        {
            var query = new GetReportsDataQuery
            {
                StartDate = startDate,
                EndDate = endDate
            };
            var result = await _mediator.Send(query);
            return Ok(result);
        }
    }

    // Request DTOs
    public class UpdateBusinessStatusRequest
    {
        public bool IsActive { get; set; }
    }

    public class UpdateAppointmentStatusRequest
    {
        public string Status { get; set; } = string.Empty;
    }

    public class UpdateAutoRenewalRequest
    {
        public bool AutoRenewal { get; set; }
    }

    public class RefundPaymentRequest
    {
        public string Reason { get; set; } = string.Empty;
    }
}
