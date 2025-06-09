using BookingSystem.Application.Interfaces.Repositories;
using BookingSystem.Domain.Entites;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BookingSystem.Infrastructure.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly BookingDbContext _context;
        private readonly ILogger<UserRepository> _logger;

        public UserRepository(
            BookingDbContext context,
            ILogger<UserRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<User> AddAsync(User user)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                _logger.LogInformation("Attempting to add new user with email {Email}", user.Email);

                bool exists = await _context.Users.AnyAsync(u => u.Email == user.Email);
                if (exists)
                {
                    _logger.LogWarning("User with email {Email} already exists", user.Email);
                    return null;
                }

                await _context.Users.AddAsync(user);
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                _logger.LogInformation("User {UserId} added successfully", user.Id);
                return user;
            }
            catch (DbUpdateException ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, "Database error while adding user");
                return null;
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, "Unexpected error while adding user");
                return null;
            }
        }

        public async Task<bool> DeleteAsync(User user)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                _logger.LogInformation("Deleting user {UserId}", user.Id);

                _context.Users.Remove(user);
                int affected = await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                if (affected > 0)
                {
                    _logger.LogInformation("User {UserId} deleted successfully", user.Id);
                    return true;
                }

                _logger.LogWarning("No records were deleted for user {UserId}", user.Id);
                return false;
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, "Error deleting user {UserId}", user.Id);
                return false;
            }
        }

        public async Task<List<User>> GetAllAsync()
        {
            try
            {
                _logger.LogInformation("Fetching all users");
                return await _context.Users.AsNoTracking().ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching users");
                return new List<User>();
            }
        }

        public async Task<User?> GetByIdAsync(int id)
        {
            try
            {
                _logger.LogDebug("Fetching user by ID {UserId}", id);
                return await _context.Users
                    .AsNoTracking()
                    .FirstOrDefaultAsync(u => u.Id == id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching user {UserId}", id);
                return null;
            }
        }

        public async Task<User> UpdateAsync(User user)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                _logger.LogInformation("Updating user {UserId}", user.Id);

                _context.Users.Update(user);
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                _logger.LogInformation("User {UserId} updated successfully", user.Id);
                return user;
            }
            catch (DbUpdateConcurrencyException ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, "Concurrency error updating user {UserId}", user.Id);
                return null;
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, "Error updating user {UserId}", user.Id);
                return null;
            }
        }

        public async Task<User?> GetByEmailAsync(string email)
        {
            try
            {
                _logger.LogDebug("Fetching user by email {Email}", email);
                return await _context.Users
                    .AsNoTracking()
                    .FirstOrDefaultAsync(u => u.Email == email);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching user by email {Email}", email);
                return null;
            }
        }
    }
}