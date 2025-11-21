using AppointmentManagementSystem.Application.DTOs;
using AppointmentManagementSystem.Application.Features.Categories.Queries;
using AppointmentManagementSystem.Application.Shared;
using AppointmentManagementSystem.Domain.Interfaces;
using AutoMapper;
using MediatR;
using System.Linq;

namespace AppointmentManagementSystem.Application.Features.Categories.Handlers
{
    public class GetAllCategoriesQueryHandler : IRequestHandler<GetAllCategoriesQuery, PaginatedResult<CategoryDto>>
    {
        private readonly ICategoryRepository _categoryRepository;
        private readonly IMapper _mapper;

        public GetAllCategoriesQueryHandler(ICategoryRepository categoryRepository, IMapper mapper)
        {
            _categoryRepository = categoryRepository;
            _mapper = mapper;
        }

        public async Task<PaginatedResult<CategoryDto>> Handle(GetAllCategoriesQuery request, CancellationToken cancellationToken)
        {
            var categories = await _categoryRepository.GetAllWithBusinessCountAsync();
            var totalCount = categories.Count();
            var paged = categories
                .Skip((request.PageNumber - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToList();

            return new PaginatedResult<CategoryDto>
            {
                Items = _mapper.Map<List<CategoryDto>>(paged),
                TotalCount = totalCount,
                PageNumber = request.PageNumber,
                PageSize = request.PageSize
            };
        }
    }
}
