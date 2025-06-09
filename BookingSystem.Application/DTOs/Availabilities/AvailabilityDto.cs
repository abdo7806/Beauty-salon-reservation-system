using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookingSystem.Application.DTOs.Availabilities
{
    public class AvailabilityDto
    {
        public int Id { get; set; }
        public int StaffId { get; set; }
        public int DayOfWeek { get; set; } // 0 = الأحد، 6 = السبت
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
    }

}
