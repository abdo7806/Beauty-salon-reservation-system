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
    public class StaffRepository : IStaffRepository
    {
        private readonly BookingDbContext _context;
        private readonly ILogger< StaffRepository> _logger;

        public StaffRepository(
            BookingDbContext context,
            ILogger< StaffRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task< Staff> AddAsync( Staff  staff)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {

                await _context.Staff.AddAsync(staff);
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                _logger.LogInformation(" Staff { StaffId} added successfully",  staff.Id);

                Staff createdStaff = await GetByIdAsync(staff.Id);
                return createdStaff;
            }
            catch (DbUpdateException ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, "Database error while adding  staff");
                return null;
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, "Unexpected error while adding  staff");
                return null;
            }
        }

        public async Task<bool> DeleteAsync(Staff  staff)
        {

            try
            {
                using (var transaction = await _context.Database.BeginTransactionAsync())
                {
                    try
                    {

                        if (staff == null)
                        {
                            _logger.LogWarning("Staff {StaffId} not found for deletion", staff.Id);

                            return false; // الوصفة غير موجودة
                        }

                        User user = staff.User;
                        if (user == null)
                        {
                            _logger.LogWarning("User {UserId} not found for deletion", user.Id);

                            return false; // لا توجد عناصر في الوصفة
                        }



                        _logger.LogInformation("Deleting  staff { StaffId}", staff.Id);

                        _context.Staff.Remove(staff);

                        _logger.LogInformation("Deleting  User { UserId}", user.Id);

                        _context.Users.Remove(user);


                        // حفظ التغييرات
                        await _context.SaveChangesAsync();

                        // تأكيد المعاملة
                        await transaction.CommitAsync();

                        return true; ;
                    }
                    catch (Exception ex)
                    {
                        // إلغاء المعاملة في حالة حدوث خطأ
                        await transaction.RollbackAsync();
                        Console.WriteLine(ex.Message);
                        return false;
                    }
                }
            }


            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting staff {StaffId}", staff.Id);
                return false;
            }

      
        }

        public async Task<List<Staff>> GetAllAsync()
        {
            try
            {
                _logger.LogInformation("Fetching all  staffs");
                return await _context.Staff
                    .Include(s => s.User)
                    .AsNoTracking().ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching  staffs");
                return new List< Staff>();
            }
        }

        public async Task< Staff?> GetByIdAsync(int id)
        {
            try
            {
                _logger.LogDebug("Fetching staff by ID { StaffId}", id);
                return await _context.Staff
                    .Include(s => s.User)
                    .AsNoTracking()
                    .FirstOrDefaultAsync(u => u.Id == id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching  staff { StaffId}", id);
                return null;
            }
        }

        public async Task<Staff> UpdateAsync(Staff  staff)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                _logger.LogInformation("Updating  staff {StaffId}",  staff.Id);

                _context.Staff.Update(staff);
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                _logger.LogInformation(" Staff { StaffId} updated successfully",  staff.Id);
                return  staff;
            }
            catch (DbUpdateConcurrencyException ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, "Concurrency error updating  staff { StaffId}",  staff.Id);
                return null;
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, "Error updating  staff { StaffId}",  staff.Id);
                return null;
            }
        }

    }
}
