using AppointmentManagementSystem.Application.DTOs;
using AppointmentManagementSystem.Application.Features.BusinessUsers.Commands;
using AppointmentManagementSystem.Domain.Entities;
using AppointmentManagementSystem.Domain.Interfaces;
using AutoMapper;
using MediatR;

namespace AppointmentManagementSystem.Application.Features.BusinessUsers.Handlers
{
    public class CreateBusinessUserCommandHandler : IRequestHandler<CreateBusinessUserCommand, BusinessUserDto>
    {
        private readonly IBusinessUserRepository _businessUserRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public CreateBusinessUserCommandHandler(
            IBusinessUserRepository businessUserRepository,
            IUnitOfWork unitOfWork,
            IMapper mapper)
        {
            _businessUserRepository = businessUserRepository;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<BusinessUserDto> Handle(CreateBusinessUserCommand request, CancellationToken cancellationToken)
        {
            var businessUser = _mapper.Map<BusinessUser>(request.CreateBusinessUserDto);
            await _businessUserRepository.AddAsync(businessUser);
            await _unitOfWork.SaveChangesAsync();

            var businessUserWithDetails = await _businessUserRepository.GetByIdWithDetailsAsync(businessUser.Id);
            return _mapper.Map<BusinessUserDto>(businessUserWithDetails);
        }
    }
}
