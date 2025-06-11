using BookingSystem.Application.DTOs.Staff;
using BookingSystem.Application.Interfaces.Repositories;
using BookingSystem.Application.Interfaces.Services;
using BookingSystem.Domain.Entites;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wasfaty.Application.DTOs.Users;

namespace BookingSystem.Application.Services
{
    public class StaffService : IStaffService
    {
        private readonly IStaffRepository _staffRepository;
        private readonly IUserRepository _userRepository;

        private readonly ILogger<StaffService> _logger;

        public StaffService(
            IStaffRepository staffRepository,
            IUserRepository userRepository,
            ILogger<StaffService> logger)
        {
            _staffRepository = staffRepository;
            _userRepository = userRepository;
            _logger = logger;
        }

        public async Task<StaffDto> CreateAsync(CreateStaffDto staffDto)
        {
            try
            {

                var staff = new Staff
                {
                    UserId = staffDto.UserId,
                    Speciality = staffDto.Speciality
                };



                var createdStaff = await _staffRepository.AddAsync(staff);

                if (createdStaff == null)
                {
                    _logger.LogWarning("Failed to create staff - StaffId {StaffId} may already exist", staffDto.UserId);
                    return null;
                }

                _logger.LogInformation("Staff {StaffId} created successfully", createdStaff.Id);
                return MapToDto(createdStaff);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating staff");
                return null;
            }
        }

        public async Task<bool> DeleteAsync(int id)
        {
            try
            {
                _logger.LogInformation("Deleting staff {StaffId}", id);

                var staff = await _staffRepository.GetByIdAsync(id);
                if (staff == null)
                {
                    _logger.LogWarning("Staff {StaffId} not found for deletion", id);
                    return false;
                }

                var result = await _staffRepository.DeleteAsync(staff);
                if (result)
                {
                    _logger.LogInformation("Staff {StaffId} deleted successfully", id);
                }
                else
                {
                    _logger.LogWarning("Failed to delete staff {StaffId}", id);
                }

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting staff {StaffId}", id);
                return false;
            }

        }

        public async Task<List<StaffDto>> GetAllAsync()
        {
            try
            {
                _logger.LogInformation("Fetching all staffs");
                var staffs = await _staffRepository.GetAllAsync();
                return staffs.Select(MapToDto).ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching staffs");
                return new List<StaffDto>();
            }
        }

        public async Task<StaffDto?> GetByIdAsync(int id)
        {
            try
            {
                _logger.LogDebug("Fetching staff {StaffId}", id);
                var staff = await _staffRepository.GetByIdAsync(id);
                if (staff == null)
                {
                    _logger.LogWarning("Staff {StaffId} not found", id);
                    return null;
                }
                return MapToDto(staff);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching staff {StaffId}", id);
                return null;
            }
        }

        public async Task<StaffDto?> UpdateAsync(int id, UpdateStaffDto staffDto)
        {
            try
            {
                _logger.LogInformation("Updating staff {StaffId}", id);

                var staff = await _staffRepository.GetByIdAsync(id);
                if (staff == null)
                {
                    _logger.LogWarning("Staff {StaffId} not found for update", id);
                    return null;
                }

                staff.Speciality = staffDto.Speciality;


                var updatedStaff = await _staffRepository.UpdateAsync(staff);
                if (updatedStaff == null)
                {
                    _logger.LogWarning("Failed to update staff {StaffId}", id);
                    return null;
                }

                _logger.LogInformation("Staff {StaffId} updated successfully", id);
                return MapToDto(updatedStaff);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating staff {StaffId}", id);
                return null;
            }
        }

        private StaffDto MapToDto(Staff staff)
        {


            return new StaffDto
            {
                Id = staff.Id,
                UserId = staff.UserId,
                Speciality = staff.Speciality,
                FullName = staff.User.FullName,
                Email = staff.User.Email,
                CreatedAt = staff.User.CreatedAt
            };
        }
    }
}

