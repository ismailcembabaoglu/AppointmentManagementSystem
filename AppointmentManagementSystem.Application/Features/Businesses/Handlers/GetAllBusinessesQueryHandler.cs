using AppointmentManagementSystem.Application.DTOs;
using AppointmentManagementSystem.Application.Features.Businesses.Queries;
using AppointmentManagementSystem.Domain.Interfaces;
using AutoMapper;
using MediatR;
using System.Linq;

namespace AppointmentManagementSystem.Application.Features.Businesses.Handlers
{
    public class GetAllBusinessesQueryHandler : IRequestHandler<GetAllBusinessesQuery, List<BusinessDto>>
    {
        private readonly IBusinessRepository _businessRepository;
        private readonly IMapper _mapper;

        public GetAllBusinessesQueryHandler(IBusinessRepository businessRepository, IMapper mapper)
        {
            _businessRepository = businessRepository;
            _mapper = mapper;
        }

        public async Task<List<BusinessDto>> Handle(GetAllBusinessesQuery request, CancellationToken cancellationToken)
        {
            var businesses = await _businessRepository.GetAllWithDetailsAsync(
                request.CategoryId, 
                request.SearchTerm,
                request.City,
                request.District
            );

            var businessDtos = new List<BusinessDto>();
            foreach (var business in businesses)
            {
                var dto = _mapper.Map<BusinessDto>(business);
                dto.AverageRating = await _businessRepository.GetAverageRatingAsync(business.Id) ?? 0;
                
                // MinRating filtresi
                if (request.MinRating.HasValue && dto.AverageRating < request.MinRating.Value)
                    continue;
                    
                businessDtos.Add(dto);
            }

            var paged = businessDtos.Skip((request.PageNumber - 1) * request.PageSize).Take(request.PageSize).ToList();
            return paged;
        }
    }
}
