using AppointmentManagementSystem.Application.DTOs;
using AppointmentManagementSystem.Application.Features.Businesses.Queries;
using AppointmentManagementSystem.Domain.Interfaces;
using AutoMapper;
using MediatR;

namespace AppointmentManagementSystem.Application.Features.Businesses.Handlers
{
    public class GetEmployeesByBusinessQueryHandler : IRequestHandler<GetEmployeesByBusinessQuery, List<EmployeeDto>>
    {
        private readonly IEmployeeRepository _employeeRepository;
        private readonly IMapper _mapper;

        public GetEmployeesByBusinessQueryHandler(IEmployeeRepository employeeRepository, IMapper mapper)
        {
            _employeeRepository = employeeRepository;
            _mapper = mapper;
        }

        public async Task<List<EmployeeDto>> Handle(GetEmployeesByBusinessQuery request, CancellationToken cancellationToken)
        {
            var employees = await _employeeRepository.GetByBusinessAsync(request.BusinessId);
            return _mapper.Map<List<EmployeeDto>>(employees);
        }
    }
}
