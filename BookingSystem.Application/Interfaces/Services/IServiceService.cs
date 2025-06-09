using BookingSystem.Application.DTOs.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookingSystem.Application.Interfaces.Services
{
    public interface IServiceService
    {
        Task<ServiceDto?> GetByIdAsync(int id);
        Task<List<ServiceDto>> GetAllAsync();
        Task CreateAsync(CreateServiceDto dto);
        Task UpdateAsync(int id, UpdateServiceDto dto);
        Task DeleteAsync(int id);
    }
}
