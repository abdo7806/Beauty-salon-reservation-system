using BookingSystem.Application.DTOs.Staff;
using BookingSystem.Domain.Entites;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;



public class AvailabilityDto
{
    public int Id { get; set; }
    public int StaffId { get; set; }
    public int DayOfWeek { get; set; } // 0 = الأحد، 6 = السبت
    public TimeSpan StartTime { get; set; }
    public TimeSpan EndTime { get; set; }


    // Navigation property
    public StaffDto Staff { get; set; } = null!;
}


