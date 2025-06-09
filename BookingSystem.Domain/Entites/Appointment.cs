using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookingSystem.Domain.Entites
{
    public class Appointment
    {
        public int Id { get; set; }

        public int ClientId { get; set; }  // FK to User (Client)
        public int StaffId { get; set; }   // FK to Staff
        public int ServiceId { get; set; } // FK to Service

        public DateTime Date { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }

        public string Status { get; set; } = "Pending"; // Pending, Confirmed, Canceled

        // Navigation properties
        public User Client { get; set; } = null!;
        public Staff Staff { get; set; } = null!;
        public Service Service { get; set; } = null!;
    }
}
