using AppointmentManagementSystem.Application.DTOs;
using AppointmentManagementSystem.Application.Features.Auth.Commands;
using AppointmentManagementSystem.Application.Shared; // BU SATIRI EKLE
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AppointmentManagementSystem.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class AuthController : ControllerBase
    {
        private readonly IMediator _mediator;

        public AuthController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("register")]
        [AllowAnonymous]
        public async Task<ActionResult<BaseResponse<AuthResponseDto>>> Register([FromBody] RegisterDto registerDto) // DÖNÜŞ TİPİ DEĞİŞTİ
        {
            var command = new RegisterCommand { RegisterDto = registerDto };
            var result = await _mediator.Send(command);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<ActionResult<BaseResponse<AuthResponseDto>>> Login([FromBody] LoginDto loginDto) // DÖNÜŞ TİPİ DEĞİŞTİ
        {
            var command = new LoginCommand { LoginDto = loginDto };
            var result = await _mediator.Send(command);
            return result.Success ? Ok(result) : BadRequest(result);
        }
    }
}
