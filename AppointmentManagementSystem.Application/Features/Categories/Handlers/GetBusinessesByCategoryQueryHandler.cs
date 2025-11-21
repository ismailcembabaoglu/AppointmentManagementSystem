using AppointmentManagementSystem.Application.DTOs;
using AppointmentManagementSystem.Application.Features.Categories.Queries;
using AppointmentManagementSystem.Application.Shared;
using AppointmentManagementSystem.Domain.Interfaces;
using AutoMapper;
using MediatR;
using System.Linq;

namespace AppointmentManagementSystem.Application.Features.Categories.Handlers
{
    public class GetBusinessesByCategoryQueryHandler : IRequestHandler<GetBusinessesByCategoryQuery, PaginatedResult<BusinessDto>>
    {
        private readonly IBusinessRepository _businessRepository;
        private readonly IMapper _mapper;

        public GetBusinessesByCategoryQueryHandler(IBusinessRepository businessRepository, IMapper mapper)
        {
            _businessRepository = businessRepository;
            _mapper = mapper;
        }

        public async Task<PaginatedResult<BusinessDto>> Handle(GetBusinessesByCategoryQuery request, CancellationToken cancellationToken)
        {
            var businesses = await _businessRepository.GetByCategoryAsync(request.CategoryId);
            var totalCount = businesses.Count();
            var paged = businesses
                .Skip((request.PageNumber - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToList();

            return new PaginatedResult<BusinessDto>
            {
                Items = _mapper.Map<List<BusinessDto>>(paged),
                TotalCount = totalCount,
                PageNumber = request.PageNumber,
                PageSize = request.PageSize
            };
        }
    }
}
