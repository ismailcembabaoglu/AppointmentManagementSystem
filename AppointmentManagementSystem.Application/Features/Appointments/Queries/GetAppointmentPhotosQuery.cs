using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppointmentManagementSystem.Application.Features.Appointments.Queries
{
    public class GetAppointmentPhotosQuery : IRequest<List<string>>
    {
        public int AppointmentId { get; set; }
    }
}
