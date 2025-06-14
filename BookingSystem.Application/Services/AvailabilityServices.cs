using BookingSystem.Application.DTOs.Availabilities;
using BookingSystem.Application.DTOs.Staff;
using BookingSystem.Application.Interfaces.Repositories;
using BookingSystem.Application.Interfaces.Services;
using BookingSystem.Domain.Entites;
using Microsoft.Extensions.Logging;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookingSystem.Application.Services
{
    
    public class AvailabilityServices : IAvailabilityService
    {
        private readonly IAvailabilityRepository _availabilityRepository;
        private readonly ILogger<AvailabilityServices> _logger;

        public AvailabilityServices(
            IAvailabilityRepository availabilityRepository,
            ILogger<AvailabilityServices> logger)
        {
            _availabilityRepository = availabilityRepository;
            _logger = logger;
        }

    

        public async Task<AvailabilityDto> CreateAsync(CreateAvailabilityDto availabilityDto)
        {
            try
            {

                var availability = new Availability
                {
                    StaffId = availabilityDto.StaffId,
                    DayOfWeek = availabilityDto.DayOfWeek,
                    StartTime = availabilityDto.StartTime,
                    EndTime = availabilityDto.EndTime,
                };

                var createdAvailability = await _availabilityRepository.AddAsync(availability);

                if (createdAvailability == null)
                {
                    _logger.LogWarning("Failed to create availability");
                    return null;
                }

                _logger.LogInformation("Availability {AvailabilityId} created successfully", createdAvailability.Id);
                return MapToDto(createdAvailability);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating availability");
                return null;
            }

        }


        public async Task<bool> DeleteAsync(int id)
        {
            try
            {
                _logger.LogInformation("Deleting availability {AvailabilityId}", id);

                var availability = await _availabilityRepository.GetByIdAsync(id);
                if (availability == null)
                {
                    _logger.LogWarning("Availability {AvailabilityId} not found for deletion", id);
                    return false;
                }

                var result = await _availabilityRepository.DeleteAsync(availability);
                if (result)
                {
                    _logger.LogInformation("Availability {AvailabilityId} deleted successfully", id);
                }
                else
                {
                    _logger.LogWarning("Failed to delete availability {AvailabilityId}", id);
                }

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting availability {AvailabilityId}", id);
                return false;
            }
        }

        public async Task<List<AvailabilityDto>> GetAllAsync()
        {
            try
            {
                _logger.LogInformation("Fetching all availabilitys");
                var availabilitys = await _availabilityRepository.GetAllAsync();
                return availabilitys.Select(MapToDto).ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching availabilitys");
                return new List<AvailabilityDto>();
            }
        }

        public async Task<AvailabilityDto?> GetByIdAsync(int id)
        {
            try
            {
                _logger.LogDebug("Fetching availability {AvailabilityId}", id);
                var availability = await _availabilityRepository.GetByIdAsync(id);
                if (availability == null)
                {
                    _logger.LogWarning("Availability {AvailabilityId} not found", id);
                    return null;
                }
                return MapToDto(availability);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching availability {AvailabilityId}", id);
                return null;
            }
       
        }

        public async Task<List<AvailabilityDto>> GetByStaffIdAsync(int staffId)
        {
            try
            {
                _logger.LogDebug("Fetching availability By staffId: {staffId}", staffId);
                var availabilitys = await _availabilityRepository.GetByStaffIdAsync(staffId);
                if (availabilitys == null)
                {
                    _logger.LogWarning("Availability By staffId: {staffId} not found", staffId);
                    return null;
                }
                return availabilitys.Select(MapToDto).ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching availability By staffId: {staffId}", staffId);
                return null;
            }
        }

        public async Task<AvailabilityDto> UpdateAsync(int id, UpdateAvailabilityDto availabilityDto)
        {
            try
            {
                _logger.LogInformation("Updating availability {AvailabilityId}", id);

                var availability = await _availabilityRepository.GetByIdAsync(id);
                if (availability == null)
                {
                    _logger.LogWarning("Availability {AvailabilityId} not found for update", id);
                    return null;
                }

                availability.DayOfWeek = availabilityDto.DayOfWeek;
                availability.StartTime = availabilityDto.StartTime;
                availability.EndTime = availabilityDto.EndTime;

                var updatedAvailability = await _availabilityRepository.UpdateAsync(availability);
                if (updatedAvailability == null)
                {
                    _logger.LogWarning("Failed to update availability {AvailabilityId}", id);
                    return null;
                }

                _logger.LogInformation("Availability {AvailabilityId} updated successfully", id);
                return MapToDto(updatedAvailability);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating availability {AvailabilityId}", id);
                return null;
            }
        }


      


        private AvailabilityDto MapToDto(Availability availability)
        {
            return new AvailabilityDto
            {
                Id = availability.Id,
                StaffId = availability.StaffId,
                DayOfWeek = availability.DayOfWeek,
                StartTime = availability.StartTime,
                EndTime = availability.EndTime,
                Staff = new StaffDto
                {
                    Id = availability.Staff.Id,
                    UserId = availability.Staff.UserId,
                    Speciality = availability.Staff.Speciality,
                    FullName = availability.Staff.User.FullName,
                    Email = availability.Staff.User.Email,
                    CreatedAt = availability.Staff.User.CreatedAt
                }
            };
        }
    }
}