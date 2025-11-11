using AppointmentManagementSystem.Application.Features.Payments.Commands;
using AppointmentManagementSystem.Application.Features.Payments.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AppointmentManagementSystem.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public PaymentsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("initiate-card-registration")]
        [AllowAnonymous]
        public async Task<IActionResult> InitiateCardRegistration([FromBody] InitiateCardRegistrationCommand command)
        {
            // Get user IP
            var userIp = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "127.0.0.1";
            command.UserIp = userIp;

            var result = await _mediator.Send(command);
            
            if (result.Success)
                return Ok(result);
            
            return BadRequest(result);
        }

        [HttpPost("webhook")]
        [AllowAnonymous]
        public async Task<IActionResult> ProcessWebhook([FromForm] ProcessPaymentWebhookCommand command)
        {
            var result = await _mediator.Send(command);
            
            // PayTR requires "OK" response
            if (result.Success)
                return Ok("OK");
            
            return BadRequest(result.Message);
        }

        [HttpGet("subscription/{businessId}")]
        [Authorize]
        public async Task<IActionResult> GetSubscription(int businessId)
        {
            var query = new GetSubscriptionByBusinessIdQuery { BusinessId = businessId };
            var result = await _mediator.Send(query);
            
            if (result.Success)
                return Ok(result);
            
            return NotFound(result);
        }

        [HttpGet("history/{businessId}")]
        [Authorize]
        public async Task<IActionResult> GetPaymentHistory(int businessId)
        {
            var query = new GetPaymentHistoryQuery { BusinessId = businessId };
            var result = await _mediator.Send(query);
            
            if (result.Success)
                return Ok(result);
            
            return BadRequest(result);
        }

        [HttpPost("webhook")]
        [AllowAnonymous]
        public async Task<IActionResult> PaymentWebhook([FromForm] ProcessPaymentWebhookCommand command)
        {
            var result = await _mediator.Send(command);
            
            if (result.Success)
                return Ok("OK");
            
            return BadRequest(result.Message);
        }
    }
}
