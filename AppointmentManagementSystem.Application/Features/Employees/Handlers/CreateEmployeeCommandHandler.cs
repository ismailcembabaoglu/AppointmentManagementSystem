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
        private readonly IBusinessRepository _businessRepository; // BU SATIR EKLENDİ
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public CreateEmployeeCommandHandler(
            IEmployeeRepository employeeRepository,
            IBusinessRepository businessRepository, // BU SATIR EKLENDİ
            IUnitOfWork unitOfWork,
            IMapper mapper)
        {
            _employeeRepository = employeeRepository;
            _businessRepository = businessRepository; // BU SATIR EKLENDİ
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<EmployeeDto> Handle(CreateEmployeeCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // Input validation
                if (request?.CreateEmployeeDto == null)
                {
                    throw new ArgumentException("Employee data is required.");
                }

                if (request.CreateEmployeeDto.BusinessId <= 0)
                {
                    throw new ArgumentException("Business ID must be greater than 0.");
                }

                // Business kontrolü
                var business = await _businessRepository.GetByIdWithDetailsAsync(request.CreateEmployeeDto.BusinessId);
                if (business == null)
                {
                    throw new Exception($"Business with ID {request.CreateEmployeeDto.BusinessId} not found.");
                }

                // Employee mapping
                var employee = _mapper.Map<Domain.Entities.Employee>(request.CreateEmployeeDto);

                // Employee business relationship
                employee.BusinessId = business.Id;
                employee.Business = business;
                await _employeeRepository.AddAsync(employee);
                await _unitOfWork.SaveChangesAsync();

                var employeeWithBusiness = await _employeeRepository.GetByIdWithBusinessAsync(employee.Id);
                if (employeeWithBusiness == null)
                {
                    throw new Exception("Employee created but could not retrieve details.");
                }

                return _mapper.Map<EmployeeDto>(employeeWithBusiness);
            }
            catch (Exception ex)
            {
                // Detaylı hata mesajı
                throw new Exception($"Error creating employee: {ex.Message}", ex);
            }
        }

    }
}
