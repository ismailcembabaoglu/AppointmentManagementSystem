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
            var resuls= new BaseResponse<AuthResponseDto>
            {
                Data = result,
                Success = !string.IsNullOrEmpty(result.Token),
                Message = !string.IsNullOrEmpty(result.Token) ? "Kayıt başarılı" : "Kayıt başarısız"
            };
            return resuls.Success? Ok(resuls) : BadRequest(result);
        }

        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<ActionResult<BaseResponse<AuthResponseDto>>> Login([FromBody] LoginDto loginDto) // DÖNÜŞ TİPİ DEĞİŞTİ
        {
            var command = new LoginCommand { LoginDto = loginDto };
            var result = await _mediator.Send(command);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpPost("register-business")]
        [AllowAnonymous]
        public async Task<ActionResult<BaseResponse<AuthResponseDto>>> RegisterBusiness([FromBody] RegisterBusinessDto registerBusinessDto)
        {
            try
            {
                var command = new RegisterBusinessCommand { RegisterBusinessDto = registerBusinessDto };
                var result = await _mediator.Send(command);
                var response = new BaseResponse<AuthResponseDto>
                {
                    Data = result,
                    Success = !string.IsNullOrEmpty(result.Token),
                    Message = !string.IsNullOrEmpty(result.Token) ? "İşletme kaydı başarılı" : "İşletme kaydı başarısız"
                };
                return response.Success ? Ok(response) : BadRequest(response);
            }
            catch (Exception ex)
            {
                var errorResponse = new BaseResponse<AuthResponseDto>
                {
                    Success = false,
                    Message = "Bir hata oluştu",
                    Errors = new List<string> { ex.Message }
                };
                return BadRequest(errorResponse);
            }
        }

        [HttpGet("verify-email")]
        [AllowAnonymous]
        public async Task<ActionResult<BaseResponse<string>>> VerifyEmail([FromQuery] string token)
        {
            try
            {
                var command = new VerifyEmailCommand { Token = token };
                var result = await _mediator.Send(command);

                var response = new BaseResponse<string>
                {
                    Data = "Email adresiniz başarıyla doğrulandı. Artık giriş yapabilirsiniz.",
                    Success = result,
                    Message = result ? "Email doğrulama başarılı" : "Email doğrulama başarısız"
                };

                return response.Success ? Ok(response) : BadRequest(response);
            }
            catch (Exception ex)
            {
                var errorResponse = new BaseResponse<string>
                {
                    Success = false,
                    Message = ex.Message,
                    Errors = new List<string> { ex.Message }
                };
                return BadRequest(errorResponse);
            }
        }
    }
}
