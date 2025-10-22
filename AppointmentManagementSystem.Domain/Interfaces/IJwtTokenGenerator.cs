using AppointmentManagementSystem.Domain.Entities;

namespace AppointmentManagementSystem.Domain.Interfaces
{
    public interface IJwtTokenGenerator
    {
        string GenerateToken(User user);
    }
}
