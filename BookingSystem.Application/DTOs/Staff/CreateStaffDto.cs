using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookingSystem.Application.DTOs.Staff
{
    public class CreateStaffDto
    {
        public int UserId { get; set; }
        public string? Speciality { get; set; }
    }
}
