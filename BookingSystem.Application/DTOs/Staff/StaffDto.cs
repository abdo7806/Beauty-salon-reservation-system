using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookingSystem.Application.DTOs.Staff
{
    public class StaffDto
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string FullName { get; set; } = default!;
        public string Email { get; set; } = default!;
        public string? Speciality { get; set; }
        public DateTime? CreatedAt { get; set; }

    }
}
