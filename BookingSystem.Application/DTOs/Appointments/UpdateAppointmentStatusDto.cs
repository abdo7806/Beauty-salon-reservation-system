using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookingSystem.Application.DTOs.Appointments
{
    public class UpdateAppointmentStatusDto
    {
        public string Status { get; set; } = default!; // "Pending", "Confirmed", "Canceled"
    }
}
