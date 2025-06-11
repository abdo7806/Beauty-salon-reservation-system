using BookingSystem.Domain.Entites;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookingSystem.Application.Interfaces.Repositories
{
    public interface IServiceRepository
    {
        Task<Service?> GetByIdAsync(int id);
        Task<List<Service>> GetAllAsync();
        Task<Service> AddAsync(Service service);
        Task<Service> UpdateAsync(Service service);
        Task<bool> DeleteAsync(Service service);
    }
}
