using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookingSystem.Domain.Entites
{
    public class Availability
    {
        public int Id { get; set; }
        public int StaffId { get; set; }  // FK to Staff
        public int DayOfWeek { get; set; } // 0=الأحد ... 6=السبت
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }

        // Navigation property
        public Staff Staff { get; set; } = null!;
    }
}
