using BookingSystem.Application.DTOs.Availabilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookingSystem.Application.Interfaces.Services
{
    public interface IAvailabilityService
    {
        Task<AvailabilityDto?> GetByIdAsync(int id);
        Task<List<AvailabilityDto>> GetAllAsync();
        Task<AvailabilityDto> CreateAsync(CreateAvailabilityDto dto);
        Task<AvailabilityDto> UpdateAsync(int id, UpdateAvailabilityDto dto);
        Task<bool> DeleteAsync(int id);
        Task<List<AvailabilityDto>> GetByStaffIdAsync(int staffId);
    }
}
