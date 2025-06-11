using BookingSystem.Application.Interfaces.Repositories;
using BookingSystem.Domain.Entites;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookingSystem.Infrastructure.Repositories
{
    public class ServiceRepository : IServiceRepository
    {
        private readonly BookingDbContext _context;
        private readonly ILogger<ServiceRepository> _logger;

        public ServiceRepository(
            BookingDbContext context,
            ILogger<ServiceRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<Service> AddAsync(Service service)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {

                await _context.Services.AddAsync(service);
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                _logger.LogInformation("Service {ServiceId} added successfully", service.Id);
                return service;
            }
            catch (DbUpdateException ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, "Database error while adding service");
                return null;
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, "Unexpected error while adding service");
                return null;
            }
        }

        public async Task<bool> DeleteAsync(Service service)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                _logger.LogInformation("Deleting service {ServiceId}", service.Id);

                _context.Services.Remove(service);
                int affected = await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                if (affected > 0)
                {
                    _logger.LogInformation("Service {ServiceId} deleted successfully", service.Id);
                    return true;
                }

                _logger.LogWarning("No records were deleted for service {ServiceId}", service.Id);
                return false;
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, "Error deleting service {ServiceId}", service.Id);
                return false;
            }
        }

        public async Task<List<Service>> GetAllAsync()
        {
            try
            {
                _logger.LogInformation("Fetching all services");
                return await _context.Services.AsNoTracking().ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching services");
                return new List<Service>();
            }
        }

        public async Task<Service?> GetByIdAsync(int id)
        {
            try
            {
                _logger.LogDebug("Fetching service by ID {ServiceId}", id);
                return await _context.Services
                    .AsNoTracking()
                    .FirstOrDefaultAsync(u => u.Id == id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching service {ServiceId}", id);
                return null;
            }
        }

        public async Task<Service> UpdateAsync(Service service)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                _logger.LogInformation("Updating service {ServiceId}", service.Id);

                _context.Services.Update(service);
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                _logger.LogInformation("Service {ServiceId} updated successfully", service.Id);
                return service;
            }
            catch (DbUpdateConcurrencyException ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, "Concurrency error updating service {ServiceId}", service.Id);
                return null;
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, "Error updating service {ServiceId}", service.Id);
                return null;
            }
        }


    }
}