using BookingSystem.Application.DTOs.Services;
using BookingSystem.Application.Interfaces.Repositories;
using BookingSystem.Application.Interfaces.Services;
using BookingSystem.Domain.Entites;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookingSystem.Application.Services
{
    public class ServiceService : IServiceService
    {
        private readonly IServiceRepository _serviceRepository;
        private readonly ILogger<ServiceService> _logger;

        public ServiceService(
            IServiceRepository serviceRepository,
            ILogger<ServiceService> logger)
        {
            _serviceRepository = serviceRepository;
            _logger = logger;
        }

        public async Task<ServiceDto?> CreateAsync(CreateServiceDto serviceDto)
        {
            try
            {

                var service = new Service
                {
                    Name = serviceDto.Name,
                    Duration = serviceDto.Duration,
                    Price = serviceDto.Price
                  
                };

                var createdService = await _serviceRepository.AddAsync(service);

                if (createdService == null)
                {
                    _logger.LogWarning("Failed to create service");
                    return null;
                }

                _logger.LogInformation("Service {ServiceId} created successfully", createdService.Id);
                return MapToDto(createdService);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating service");
                return null;
            }
        }

        public async Task<bool> DeleteAsync(int id)
        {
            try
            {
                _logger.LogInformation("Deleting service {ServiceId}", id);

                var service = await _serviceRepository.GetByIdAsync(id);
                if (service == null)
                {
                    _logger.LogWarning("Service {ServiceId} not found for deletion", id);
                    return false;
                }

                var result = await _serviceRepository.DeleteAsync(service);
                if (result)
                {
                    _logger.LogInformation("Service {ServiceId} deleted successfully", id);
                }
                else
                {
                    _logger.LogWarning("Failed to delete service {ServiceId}", id);
                }

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting service {ServiceId}", id);
                return false;
            }

        }

        public async Task<List<ServiceDto>> GetAllAsync()
        {
            try
            {
                _logger.LogInformation("Fetching all services");
                var services = await _serviceRepository.GetAllAsync();
                return services.Select(MapToDto).ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching services");
                return new List<ServiceDto>();
            }
        }

        public async Task<ServiceDto?> GetByIdAsync(int id)
        {
            try
            {
                _logger.LogDebug("Fetching service {ServiceId}", id);
                var service = await _serviceRepository.GetByIdAsync(id);
                if (service == null)
                {
                    _logger.LogWarning("Service {ServiceId} not found", id);
                    return null;
                }
                return MapToDto(service);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching service {ServiceId}", id);
                return null;
            }
        }

        public async Task<ServiceDto?> UpdateAsync(int id, UpdateServiceDto serviceDto)
        {
            try
            {
                _logger.LogInformation("Updating service {ServiceId}", id);

                var service = await _serviceRepository.GetByIdAsync(id);
                if (service == null)
                {
                    _logger.LogWarning("Service {ServiceId} not found for update", id);
                    return null;
                }

                service.Name = serviceDto.Name;
                service.Duration = serviceDto.Duration;
                service.Price = serviceDto.Price;

                var updatedService = await _serviceRepository.UpdateAsync(service);
                if (updatedService == null)
                {
                    _logger.LogWarning("Failed to update service {ServiceId}", id);
                    return null;
                }

                _logger.LogInformation("Service {ServiceId} updated successfully", id);
                return MapToDto(updatedService);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating service {ServiceId}", id);
                return null;
            }
        }

        private ServiceDto MapToDto(Service service)
        {
            return new ServiceDto
            {
                Id = service.Id,
                Name = service.Name,
                Duration = service.Duration,
                Price = service.Price
            };
        }
    }
}