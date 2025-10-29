using AppointmentManagementSystem.Application.DTOs;
using AppointmentManagementSystem.Application.Features.Photos.Commands;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AppointmentManagementSystem.API.Controllers
{
    [Authorize]
    public class PhotosController : BaseController
    {
        private readonly IMediator _mediator;

        public PhotosController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Upload photo for a business
        /// </summary>
        [HttpPost("business/{businessId:int}")]
        [Authorize(Roles = "Business,Admin")]
        public async Task<IActionResult> UploadBusinessPhoto(int businessId, [FromBody] UploadPhotoDto photoDto)
        {
            try
            {
                var command = new UploadBusinessPhotoCommand
                {
                    BusinessId = businessId,
                    PhotoDto = photoDto
                };
                var result = await _mediator.Send(command);
                return OkResponse(result, "Fotoğraf başarıyla yüklendi.");
            }
            catch (Exception ex)
            {
                return ErrorResponse<PhotoDto>(ex.Message, new List<string> { ex.Message });
            }
        }

        /// <summary>
        /// Upload photo for an employee
        /// </summary>
        [HttpPost("employee/{employeeId:int}")]
        [Authorize(Roles = "Business,Admin")]
        public async Task<IActionResult> UploadEmployeePhoto(int employeeId, [FromBody] UploadPhotoDto photoDto)
        {
            try
            {
                var command = new UploadEmployeePhotoCommand
                {
                    EmployeeId = employeeId,
                    PhotoDto = photoDto
                };
                var result = await _mediator.Send(command);
                return OkResponse(result, "Fotoğraf başarıyla yüklendi.");
            }
            catch (Exception ex)
            {
                return ErrorResponse<PhotoDto>(ex.Message, new List<string> { ex.Message });
            }
        }

        /// <summary>
        /// Upload photo for a service
        /// </summary>
        [HttpPost("service/{serviceId:int}")]
        [Authorize(Roles = "Business,Admin")]
        public async Task<IActionResult> UploadServicePhoto(int serviceId, [FromBody] UploadPhotoDto photoDto)
        {
            try
            {
                var command = new UploadServicePhotoCommand
                {
                    ServiceId = serviceId,
                    PhotoDto = photoDto
                };
                var result = await _mediator.Send(command);
                return OkResponse(result, "Fotoğraf başarıyla yüklendi.");
            }
            catch (Exception ex)
            {
                return ErrorResponse<PhotoDto>(ex.Message, new List<string> { ex.Message });
            }
        }

        /// <summary>
        /// Delete a photo by ID
        /// </summary>
        [HttpDelete("{photoId:int}")]
        [Authorize(Roles = "Business,Admin")]
        public async Task<IActionResult> DeletePhoto(int photoId)
        {
            try
            {
                var command = new DeletePhotoCommand { PhotoId = photoId };
                var result = await _mediator.Send(command);

                if (!result)
                    return ErrorResponse<bool>("Fotoğraf bulunamadı.", new List<string> { "Geçersiz fotoğraf ID" });

                return OkResponse(result, "Fotoğraf başarıyla silindi.");
            }
            catch (Exception ex)
            {
                return ErrorResponse<bool>(ex.Message, new List<string> { ex.Message });
            }
        }
    }
}
