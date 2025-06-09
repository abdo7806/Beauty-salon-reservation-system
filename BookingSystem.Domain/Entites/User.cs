using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookingSystem.Domain.Entites
{
    public enum UserRole
    {
        Client,
        Staff,
        Admin
    }

    public class User
    {
        public int Id { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public UserRole Role { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation properties
        public Staff? StaffProfile { get; set; }  // فقط إذا كان موظف
        public ICollection<Appointment> ClientAppointments { get; set; } = new List<Appointment>();
    }
}
