using AppointmentManagementSystem.Application.DTOs;
using AppointmentManagementSystem.Application.Features.Businesses.Commands;
using AppointmentManagementSystem.Domain.Entities;
using AppointmentManagementSystem.Domain.Interfaces;
using AutoMapper;
using MediatR;

namespace AppointmentManagementSystem.Application.Features.Businesses.Handlers
{
    public class CreateBusinessCommandHandler : IRequestHandler<CreateBusinessCommand, BusinessDto>
    {
        private readonly IBusinessRepository _businessRepository;
        private readonly ICategoryRepository _categoryRepository; // BU SATIR EKLENDİ
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public CreateBusinessCommandHandler(
            IBusinessRepository businessRepository,
            ICategoryRepository categoryRepository, // BU SATIR EKLENDİ
            IUnitOfWork unitOfWork,
            IMapper mapper)
        {
            _businessRepository = businessRepository;
            _categoryRepository = categoryRepository; // BU SATIR EKLENDİ
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<BusinessDto> Handle(CreateBusinessCommand request, CancellationToken cancellationToken)
        {
            // Category kontrolü
            var category = await _categoryRepository.GetByIdAsync(request.CreateBusinessDto.CategoryId);
            if (category == null)
            {
                throw new Exception($"Category with ID {request.CreateBusinessDto.CategoryId} not found.");
            }

            var business = _mapper.Map<Business>(request.CreateBusinessDto);
            await _businessRepository.AddAsync(business);
            await _unitOfWork.SaveChangesAsync();

            var businessWithCategory = await _businessRepository.GetByIdWithDetailsAsync(business.Id);
            return _mapper.Map<BusinessDto>(businessWithCategory);
        }
    }
}
