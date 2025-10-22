using AppointmentManagementSystem.Application.DTOs;
using AppointmentManagementSystem.Application.Features.Businesses.Queries;
using AppointmentManagementSystem.Domain.Interfaces;
using AutoMapper;
using MediatR;

namespace AppointmentManagementSystem.Application.Features.Businesses.Handlers
{
    public class GetBusinessByIdQueryHandler : IRequestHandler<GetBusinessByIdQuery, BusinessDto?>
    {
        private readonly IBusinessRepository _businessRepository;
        private readonly IMapper _mapper;

        public GetBusinessByIdQueryHandler(IBusinessRepository businessRepository, IMapper mapper)
        {
            _businessRepository = businessRepository;
            _mapper = mapper;
        }

        public async Task<BusinessDto?> Handle(GetBusinessByIdQuery request, CancellationToken cancellationToken)
        {
            var business = await _businessRepository.GetByIdWithDetailsAsync(request.Id);
            if (business == null)
                return null;

            var dto = _mapper.Map<BusinessDto>(business);
            dto.AverageRating = await _businessRepository.GetAverageRatingAsync(business.Id) ?? 0;
            return dto;
        }
    }
}
