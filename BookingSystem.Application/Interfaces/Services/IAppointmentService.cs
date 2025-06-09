using BookingSystem.Application.DTOs.Appointments;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookingSystem.Application.Interfaces.Services
{
    public interface IAppointmentService
    {
        Task<AppointmentDto?> GetByIdAsync(int id);
        Task<List<AppointmentDto>> GetAllAsync();
        Task CreateAsync(CreateAppointmentDto dto);
        Task UpdateStatusAsync(int id, string status);
        Task DeleteAsync(int id);
        Task<List<AppointmentDto>> GetByClientIdAsync(int clientId);
        Task<List<AppointmentDto>> GetByStaffIdAsync(int staffId);
    }
}
