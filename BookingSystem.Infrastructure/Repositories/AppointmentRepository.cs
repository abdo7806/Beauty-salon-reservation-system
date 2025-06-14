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
    public class AppointmentRepository : IAppointmentRepository
    {
        private readonly BookingDbContext _context;
        private readonly ILogger<AppointmentRepository> _logger;

        public AppointmentRepository(
            BookingDbContext context,
            ILogger<AppointmentRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<Appointment> AddAsync(Appointment appointment)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {


                await _context.Appointments.AddAsync(appointment);
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                _logger.LogInformation("Appointment {AppointmentId} added successfully", appointment.Id);
                return appointment;
            }
            catch (DbUpdateException ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, "Database error while adding appointment");
                return null;
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, "Unexpected error while adding appointment");
                return null;
            }

        }

        public async Task<bool> DeleteAsync(Appointment appointment)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                _logger.LogInformation("Deleting appointment {AppointmentId}", appointment.Id);

                _context.Appointments.Remove(appointment);
                int affected = await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                if (affected > 0)
                {
                    _logger.LogInformation("Appointment {AppointmentId} deleted successfully", appointment.Id);
                    return true;
                }

                _logger.LogWarning("No records were deleted for appointment {AppointmentId}", appointment.Id);
                return false;
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, "Error deleting appointment {AppointmentId}", appointment.Id);
                return false;
            }

        }

        public async Task<List<Appointment>> GetAllAsync()
        {
            try
            {
                _logger.LogInformation("Fetching all appointments");
                return await _context.Appointments.AsNoTracking().ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching appointments");
                return new List<Appointment>();
            }
        }

        public async Task<List<Appointment>> GetByClientIdAsync(int clientId)
        {

            try
            {
                _logger.LogDebug("Fetching appointments by ID {clientId}", clientId);
                return await _context.Appointments
                    .Include(a => a.Client)
                    .Include(a => a.Staff)
                    .Include(a => a.Service)
                    .AsNoTracking()
                    .Where(a => a.ClientId == clientId)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching appointments by ID {clientId}", clientId);
                return null;
            }
        }

        public async Task<Appointment?> GetByIdAsync(int id)
        {
            try
            {
                _logger.LogDebug("Fetching appointment by ID {AppointmentId}", id);
                return await _context.Appointments
                    .AsNoTracking()
                    .FirstOrDefaultAsync(u => u.Id == id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching appointment {AppointmentId}", id);
                return null;
            }
        }

        // التحقق من عدم وجود موعد بنفس الوقت
        public async Task<List<Appointment>> GetByStaffIdAndDateAsync(int staffId, DateTime date)
        {
            return await _context.Appointments
                 .Where(a => a.StaffId == staffId && a.Date.Date == date.Date)
                 .ToListAsync();
        }

        public async Task<List<Appointment>> GetByStaffIdAsync(int staffId)
        {
            try
            {
                _logger.LogDebug("Fetching appointments by ID {staffId}", staffId);
                return await _context.Appointments
                    .Include(a => a.Client)
                    .Include(a => a.Staff)
                    .Include(a => a.Service)
                    .AsNoTracking()
                    .Where(a => a.StaffId == staffId)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching appointments by ID {staffId}", staffId);
                return null;
            };
        }

        public async Task<Appointment> UpdateAsync(Appointment appointment)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                _logger.LogInformation("Updating appointment {AppointmentId}", appointment.Id);

                _context.Appointments.Update(appointment);
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                _logger.LogInformation("Appointment {AppointmentId} updated successfully", appointment.Id);
                return appointment;
            }
            catch (DbUpdateConcurrencyException ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, "Concurrency error updating appointment {AppointmentId}", appointment.Id);
                return null;
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, "Error updating appointment {AppointmentId}", appointment.Id);
                return null;
            }

        }


    }
}
