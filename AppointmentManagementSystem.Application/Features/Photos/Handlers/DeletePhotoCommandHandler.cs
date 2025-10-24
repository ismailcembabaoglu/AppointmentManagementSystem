using AppointmentManagementSystem.Application.Features.Photos.Commands;
using AppointmentManagementSystem.Domain.Interfaces;
using AppointmentManagementSystem.Infrastructure.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AppointmentManagementSystem.Application.Features.Photos.Handlers
{
    public class DeletePhotoCommandHandler : IRequestHandler<DeletePhotoCommand, bool>
    {
        private readonly AppointmentDbContext _context;
        private readonly IUnitOfWork _unitOfWork;

        public DeletePhotoCommandHandler(AppointmentDbContext context, IUnitOfWork unitOfWork)
        {
            _context = context;
            _unitOfWork = unitOfWork;
        }

        public async Task<bool> Handle(DeletePhotoCommand request, CancellationToken cancellationToken)
        {
            // Find photo in Photos table (TPH - Table Per Hierarchy)
            var photo = await _context.Set<Domain.Entities.Photo>()
                .FirstOrDefaultAsync(p => p.Id == request.PhotoId, cancellationToken);

            if (photo == null)
                return false;

            // Soft delete
            photo.IsDeleted = true;
            await _unitOfWork.SaveChangesAsync();

            return true;
        }
    }
}
