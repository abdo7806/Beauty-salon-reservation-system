using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookingSystem.Application.DTOs.Appointments
{
    public class CreateAppointmentDto
    {
        public int ClientId { get; set; }
        public int StaffId { get; set; }
        public int ServiceId { get; set; }

        public DateTime Date { get; set; }
        public TimeSpan StartTime { get; set; }
    }

}
