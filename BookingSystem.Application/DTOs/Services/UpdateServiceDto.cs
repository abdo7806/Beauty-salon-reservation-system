using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookingSystem.Application.DTOs.Services
{
    public class UpdateServiceDto
    {
        public string Name { get; set; } = default!;
        public int Duration { get; set; }
        public decimal Price { get; set; }
    }
}
