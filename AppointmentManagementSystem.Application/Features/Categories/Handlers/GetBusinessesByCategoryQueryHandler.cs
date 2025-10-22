using AppointmentManagementSystem.Application.DTOs;
using AppointmentManagementSystem.Application.Features.Categories.Queries;
using AppointmentManagementSystem.Domain.Interfaces;
using AutoMapper;
using MediatR;

namespace AppointmentManagementSystem.Application.Features.Categories.Handlers
{
    public class GetBusinessesByCategoryQueryHandler : IRequestHandler<GetBusinessesByCategoryQuery, List<BusinessDto>>
    {
        private readonly IBusinessRepository _businessRepository;
        private readonly IMapper _mapper;

        public GetBusinessesByCategoryQueryHandler(IBusinessRepository businessRepository, IMapper mapper)
        {
            _businessRepository = businessRepository;
            _mapper = mapper;
        }

        public async Task<List<BusinessDto>> Handle(GetBusinessesByCategoryQuery request, CancellationToken cancellationToken)
        {
            var businesses = await _businessRepository.GetByCategoryAsync(request.CategoryId);
            return _mapper.Map<List<BusinessDto>>(businesses);
        }
    }
}
