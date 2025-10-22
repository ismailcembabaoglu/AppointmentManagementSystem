using AppointmentManagementSystem.Application.DTOs;
using AppointmentManagementSystem.Application.Features.Employees.Commands;
using AppointmentManagementSystem.Domain.Interfaces;
using AutoMapper;
using MediatR;

namespace AppointmentManagementSystem.Application.Features.Employees.Handlers
{
    public class CreateEmployeeCommandHandler : IRequestHandler<CreateEmployeeCommand, EmployeeDto>
    {
        private readonly IEmployeeRepository _employeeRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public CreateEmployeeCommandHandler(
            IEmployeeRepository employeeRepository,
            IUnitOfWork unitOfWork,
            IMapper mapper)
        {
            _employeeRepository = employeeRepository;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<EmployeeDto> Handle(CreateEmployeeCommand request, CancellationToken cancellationToken)
        {
            var employee = _mapper.Map<Domain.Entities.Employee>(request.CreateEmployeeDto);
            await _employeeRepository.AddAsync(employee);
            await _unitOfWork.SaveChangesAsync();

            var employeeWithBusiness = await _employeeRepository.GetByIdWithBusinessAsync(employee.Id);
            return _mapper.Map<EmployeeDto>(employeeWithBusiness);
        }
    }
}
