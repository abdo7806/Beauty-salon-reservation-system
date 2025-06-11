using BookingSystem.Domain.Entites;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookingSystem.Application.Interfaces.Repositories
{
    public interface IStaffRepository
    {
        Task<Staff?> GetByIdAsync(int id);
        Task<List<Staff>> GetAllAsync();
        Task<Staff> AddAsync(Staff staff);
        Task<Staff> UpdateAsync(Staff staff);
        Task<bool> DeleteAsync(Staff staff);
    }
}
