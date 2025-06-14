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
        Task<AppointmentDto> CreateAsync(CreateAppointmentDto dto);
        Task<AppointmentDto> UpdateStatusAsync(int id, string status);
        Task<bool> DeleteAsync(int id);
        Task<List<AppointmentDto>> GetByClientIdAsync(int clientId);
        Task<List<AppointmentDto>> GetByStaffIdAsync(int staffId);
    }
}
