using BookingSystem.Domain.Entites;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookingSystem.Application.Interfaces.Repositories
{
    public interface IAvailabilityRepository
    {
        Task<Availability?> GetByIdAsync(int id);
        Task<List<Availability>> GetAllAsync();
        Task<List<Availability>> GetByStaffIdAsync(int staffId);
        Task<Availability> AddAsync(Availability availability);
        Task<Availability> UpdateAsync(Availability availability);
        Task<bool> DeleteAsync(Availability availability);

        Task<Availability?> GetByStaffIdAndDayAsync(int staffId, DayOfWeek day);


    }
}
