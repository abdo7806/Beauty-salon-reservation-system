using BookingSystem.Application.DTOs.Staff;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookingSystem.Application.Interfaces.Services
{
    public interface IStaffService
    {
        Task<StaffDto?> GetByIdAsync(int id);
        Task<List<StaffDto>> GetAllAsync();
        Task CreateAsync(CreateStaffDto dto);
        Task UpdateAsync(int id, UpdateStaffDto dto);
        Task DeleteAsync(int id);
    }
}
