using BookingSystem.Domain.Entites;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookingSystem.Application.Interfaces.Repositories
{
    public interface IAppointmentRepository
    {
        Task<Appointment?> GetByIdAsync(int id);
        Task<List<Appointment>> GetAllAsync();
        Task<List<Appointment>> GetByClientIdAsync(int clientId);
        Task<List<Appointment>> GetByStaffIdAsync(int staffId);
        Task<Appointment> AddAsync(Appointment appointment);
        Task<Appointment> UpdateAsync(Appointment appointment);
        Task<bool> DeleteAsync(Appointment appointment);

        Task<List<Appointment>> GetByStaffIdAndDateAsync(int staffId, DateTime date);

    }
}
