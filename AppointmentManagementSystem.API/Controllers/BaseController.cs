using AppointmentManagementSystem.Application.Shared;
using Microsoft.AspNetCore.Mvc;

namespace AppointmentManagementSystem.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BaseController : ControllerBase
    {
        protected IActionResult OkResponse<T>(T data, string message = "")
        {
            return Ok(BaseResponse<T>.SuccessResponse(data, message));
        }

        protected IActionResult ErrorResponse<T>(string message, List<string>? errors = null)
        {
            return BadRequest(BaseResponse<T>.ErrorResponse(message, errors));
        }

        protected IActionResult ValidationErrorResponse<T>(List<string> errors)
        {
            return BadRequest(BaseResponse<T>.ValidationErrorResponse(errors));
        }
    }
}
