using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookingSystem.Application.DTOs.Services
{
    public class ServiceDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = default!;
        public int Duration { get; set; } // بالدقائق
        public decimal Price { get; set; }
    }
}
