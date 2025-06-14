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
    public class AvailabilityRespository : IAvailabilityRepository
    {
        private readonly BookingDbContext _context;
        private readonly ILogger<AvailabilityRespository> _logger;

        public AvailabilityRespository(
            BookingDbContext context,
            ILogger<AvailabilityRespository> logger)
        {
            _context = context;
            _logger = logger;
        }
        public async Task<Availability> AddAsync(Availability availability)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
       

                await _context.Availabilities.AddAsync(availability);
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                _logger.LogInformation("Availability {AvailabilityId} added successfully", availability.Id);
                return availability;
            }
            catch (DbUpdateException ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, "Database error while adding availability");
                return null;
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, "Unexpected error while adding availability");
                return null;
            }
        }

        public async Task<bool> DeleteAsync(Availability availability)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                _logger.LogInformation("Deleting availability {AvailabilityId}", availability.Id);

                _context.Availabilities.Remove(availability);
                int affected = await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                if (affected > 0)
                {
                    _logger.LogInformation("Availability {AvailabilityId} deleted successfully", availability.Id);
                    return true;
                }

                _logger.LogWarning("No records were deleted for availability {AvailabilityId}", availability.Id);
                return false;
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, "Error deleting availability {AvailabilityId}", availability.Id);
                return false;
            }
        }

        public async Task<List<Availability>> GetAllAsync()
        {
            try
            {
                _logger.LogInformation("Fetching all availabilities");
                return await _context.Availabilities
                    .Include(a => a.Staff)
                    .Include(s => s.Staff.User)
                    .AsNoTracking().ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching availabilitys");
                return new List<Availability>();
            }
        }

        public async Task<Availability?> GetByIdAsync(int id)
        {
            try
            {
                _logger.LogDebug("Fetching availability by ID {AvailabilityId}", id);
                return await _context.Availabilities
                    .Include(a => a.Staff)
                    .Include(s => s.Staff.User)
                    .AsNoTracking()
                    .FirstOrDefaultAsync(a => a.Id == id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching availability {AvailabilityId}", id);
                return null;
            }
        }

        public async Task<Availability?> GetByStaffIdAndDayAsync(int staffId, DayOfWeek day)
        {

            try
            {
                int d = (int)day;
                _logger.LogDebug("Fetching Availabilities by staffId {staffId} and DayAsync{day}", staffId, day);
                var data = await _context.Availabilities
               .FirstOrDefaultAsync(a => a.StaffId == staffId && a.DayOfWeek == d);
                _logger.LogDebug("Fetching Availabilities by staffId {staffId} and DayAsync{day}", staffId, day);

                return data;

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching Availabilities by staffId {staffId} and DayAsync{day}", staffId, day);
                return null;
            }
  
        }

        public async Task<List<Availability>> GetByStaffIdAsync(int staffId)
        {
            try
            {
                _logger.LogDebug("Fetching Availabilities by staffId {staffId}", staffId);
                return await _context.Availabilities
                  .Include(a => a.Staff)
                    .Include(s => s.Staff.User)
                  .AsNoTracking()
                  .Where(a => a.StaffId == staffId)
                  .ToListAsync();
            
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching Availabilities {staffId}", staffId);
                return null;
            }
        }

        public async Task<Availability> UpdateAsync(Availability availability)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                _logger.LogInformation("Updating availability {AvailabilityId}", availability.Id);

                _context.Availabilities.Update(availability);
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                _logger.LogInformation("Availability {AvailabilityId} updated successfully", availability.Id);
                return availability;
            }
            catch (DbUpdateConcurrencyException ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, "Concurrency error updating availability {AvailabilityId}", availability.Id);
                return null;
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, "Error updating availability {AvailabilityId}", availability.Id);
                return null;
            }
        }
    }
}
