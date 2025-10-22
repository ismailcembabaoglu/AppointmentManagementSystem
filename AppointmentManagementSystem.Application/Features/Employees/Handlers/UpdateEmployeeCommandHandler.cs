using AppointmentManagementSystem.Application.DTOs;
using AppointmentManagementSystem.Application.Features.Employees.Commands;
using AppointmentManagementSystem.Domain.Interfaces;
using AutoMapper;
using MediatR;

namespace AppointmentManagementSystem.Application.Features.Employees.Handlers
{
    public class UpdateEmployeeCommandHandler : IRequestHandler<UpdateEmployeeCommand, EmployeeDto?>
    {
        private readonly IEmployeeRepository _employeeRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public UpdateEmployeeCommandHandler(
            IEmployeeRepository employeeRepository,
            IUnitOfWork unitOfWork,
            IMapper mapper)
        {
            _employeeRepository = employeeRepository;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<EmployeeDto?> Handle(UpdateEmployeeCommand request, CancellationToken cancellationToken)
        {
            var employee = await _employeeRepository.GetByIdAsync(request.Id);
            if (employee == null)
                return null;

            _mapper.Map(request.CreateEmployeeDto, employee);
            employee.UpdatedAt = DateTime.UtcNow;

            await _employeeRepository.UpdateAsync(employee);
            await _unitOfWork.SaveChangesAsync();

            var employeeWithBusiness = await _employeeRepository.GetByIdWithBusinessAsync(employee.Id);
            return employeeWithBusiness != null ? _mapper.Map<EmployeeDto>(employeeWithBusiness) : null;
        }
    }
}
