using AppointmentManagementSystem.API.Controllers;
using AppointmentManagementSystem.Application.DTOs;
using AppointmentManagementSystem.Application.Features.Categories.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AppointmentManagementSystem.API.Controllers
{
    public class CategoriesController : BaseController
    {
        private readonly IMediator _mediator;

        public CategoriesController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetAll()
        {
            var query = new GetAllCategoriesQuery();
            var result = await _mediator.Send(query);
            return OkResponse(result, "Kategoriler başarıyla getirildi.");
        }

        [HttpGet("{id:int}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetById(int id)
        {
            var query = new GetCategoryByIdQuery { Id = id };
            var result = await _mediator.Send(query);

            if (result == null)
                return ErrorResponse<CategoryDto>("Kategori bulunamadı.", new List<string> { "Geçersiz kategori ID" });

            return OkResponse(result, "Kategori başarıyla getirildi.");
        }

        [HttpGet("{id:int}/businesses")]
        [AllowAnonymous]
        public async Task<IActionResult> GetBusinessesByCategory(int id)
        {
            var query = new GetBusinessesByCategoryQuery { CategoryId = id };
            var result = await _mediator.Send(query);
            return OkResponse(result, "Kategoriye ait işletmeler başarıyla getirildi.");
        }
    }
}
