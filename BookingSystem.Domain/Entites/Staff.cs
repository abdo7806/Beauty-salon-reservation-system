using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookingSystem.Domain.Entites
{
    public class Staff
    {
        public int Id { get; set; }
        public int UserId { get; set; }  // FK to User
        public string? Speciality { get; set; }

        // Navigation properties
        public User User { get; set; } = null!;
        public ICollection<Availability> Availabilities { get; set; } = new List<Availability>();
        public ICollection<Appointment> StaffAppointments { get; set; } = new List<Appointment>();
    }
}
