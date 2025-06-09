
using BookingSystem.Application.Interfaces.Repositories;
using BookingSystem.Application.Interfaces.Services;
using BookingSystem.Domain.Entites;

using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Wasfaty.Application.DTOs.Users;

namespace BookingSystem.Application.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly ILogger<UserService> _logger;

        public UserService(
            IUserRepository userRepository,
            ILogger<UserService> logger)
        {
            _userRepository = userRepository;
            _logger = logger;
        }

        public async Task<UserDto?> CreateAsync(CreateUserDto userDto)
        {
            try
            {
                _logger.LogInformation("Creating new user with email {Email}", userDto.Email);

                var user = new User
                {
                    FullName = userDto.FullName,
                    Email = userDto.Email,
                    Role = (UserRole)userDto.Role,
                    CreatedAt = DateTime.UtcNow,
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword(userDto.Password)
                };

                var createdUser = await _userRepository.AddAsync(user);

                if (createdUser == null)
                {
                    _logger.LogWarning("Failed to create user - email {Email} may already exist", userDto.Email);
                    return null;
                }

                _logger.LogInformation("User {UserId} created successfully", createdUser.Id);
                return MapToDto(createdUser);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating user");
                return null;
            }
        }

        public async Task<bool> DeleteAsync(int id)
        {
            try
            {
                _logger.LogInformation("Deleting user {UserId}", id);

                var user = await _userRepository.GetByIdAsync(id);
                if (user == null)
                {
                    _logger.LogWarning("User {UserId} not found for deletion", id);
                    return false;
                }

                var result = await _userRepository.DeleteAsync(user);
                if (result)
                {
                    _logger.LogInformation("User {UserId} deleted successfully", id);
                }
                else
                {
                    _logger.LogWarning("Failed to delete user {UserId}", id);
                }

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting user {UserId}", id);
                return false;
            }
        }

        public async Task<List<UserDto>> GetAllAsync()
        {
            try
            {
                _logger.LogInformation("Fetching all users");
                var users = await _userRepository.GetAllAsync();
                return users.Select(MapToDto).ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching users");
                return new List<UserDto>();
            }
        }

        public async Task<UserDto?> GetByIdAsync(int id)
        {
            try
            {
                _logger.LogDebug("Fetching user {UserId}", id);
                var user = await _userRepository.GetByIdAsync(id);
                if (user == null)
                {
                    _logger.LogWarning("User {UserId} not found", id);
                    return null;
                }
                return MapToDto(user);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching user {UserId}", id);
                return null;
            }
        }

        public async Task<UserDto?> UpdateAsync(int id, UpdateUserDto userDto)
        {
            try
            {
                _logger.LogInformation("Updating user {UserId}", id);

                var user = await _userRepository.GetByIdAsync(id);
                if (user == null)
                {
                    _logger.LogWarning("User {UserId} not found for update", id);
                    return null;
                }

                user.FullName = userDto.FullName;
                user.Email = userDto.Email;
                user.Role = (UserRole)userDto.Role;

                var updatedUser = await _userRepository.UpdateAsync(user);
                if (updatedUser == null)
                {
                    _logger.LogWarning("Failed to update user {UserId}", id);
                    return null;
                }

                _logger.LogInformation("User {UserId} updated successfully", id);
                return MapToDto(updatedUser);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating user {UserId}", id);
                return null;
            }
        }

        private UserDto MapToDto(User user)
        {
            return new UserDto
            {
                Id = user.Id,
                FullName = user.FullName,
                Email = user.Email,
                Role = user.Role.ToString(),
                CreatedAt = user.CreatedAt
            };
        }
    }
}