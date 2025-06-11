using BookingSystem.Application.DTOs.Services;
using BookingSystem.Application.Interfaces.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace BookingSystem.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ServiceController : ControllerBase
    {
        private readonly IServiceService _serviceService;
        private readonly ILogger<ServiceController> _logger;

        public ServiceController(
            IServiceService serviceService,
            ILogger<ServiceController> logger)
        {
            _serviceService = serviceService;
            _logger = logger;
        }


        // GET: api/service
        /// <summary>
        /// Retrieves all services
        /// </summary>
        /// <response code="200">Returns the list of services</response>
        /// <response code="204">No services found</response>
        [HttpGet]
        [ProducesResponseType(typeof(List<ServiceDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<List<ServiceDto>>> GetAllServices()
        {
            _logger.LogInformation("Fetching all services");
            var services = await _serviceService.GetAllAsync();

            if (!services.Any())
            {
                _logger.LogInformation("No services found");
                return NoContent();
            }
            return Ok(services);
        }

        // GET: api/service/{id}
        /// <summary>
        /// Gets a specific service by ID
        /// </summary>
        /// <param name="id">Service ID</param>
        /// <response code="200">Returns the requested service</response>
        /// <response code="404">Service not found</response>
        [HttpGet("{id:int:min(1)}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ServiceDto>> GetServiceById([FromRoute] int id)
        {
            _logger.LogInformation("Fetching service with ID {ServiceId}", id);

            var service = await _serviceService.GetByIdAsync(id);
            if (service == null)
            {
                _logger.LogWarning("Service with ID {ServiceId} not found", id);
                return NotFound("Service not found");
            }

            return Ok(service);
        }

        /// <summary>
        /// Creates a new service
        /// </summary>
        /// <param name="serviceDto">Service data</param>
        /// <response code="201">Service created successfully</response>
        /// <response code="400">Invalid input data</response>
        // POST api/service
        [HttpPost]
        [ProducesResponseType(typeof(ServiceDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult> CreateService([FromBody, Required] CreateServiceDto serviceDto)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Invalid model state for service creation");
                return BadRequest(ModelState);
            }


            var result = await _serviceService.CreateAsync(serviceDto);

            if (result == null)
            {
                return BadRequest("Servicename already exists");
            }
            _logger.LogInformation("Service {ServiceId} created successfully", result.Id);

            return CreatedAtAction(nameof(GetServiceById), new { id = result.Id }, result);

        }



        // PUT: api/service/{id}
        /// <summary>
        /// Updates an existing service
        /// </summary>
        /// <param name="id">Service ID</param>
        /// <param name="serviceDto">Updated service data</param>
        /// <response code="200">Service updated successfully</response>
        /// <response code="400">Invalid input data</response>
        /// <response code="404">Service not found</response>
        [HttpPut("{id:int:min(1)}")]
        [ProducesResponseType(typeof(ServiceDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ServiceDto>> UpdateService([FromRoute] int id, [FromBody, Required] UpdateServiceDto serviceDto)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Invalid model state for service update");
                return BadRequest(ModelState);
            }
            _logger.LogInformation("Updating service {ServiceId}", id);

            var result = await _serviceService.UpdateAsync(id, serviceDto);
            if (result == null)
            {
                _logger.LogWarning("Service {ServiceId} not found for update", id);
                return NotFound("Service not found");
            }
            _logger.LogInformation("Service {ServiceId} updated successfully", id);

            return Ok(result);
        }

        // DELETE: api/service/{id}
        /// <summary>
        /// Deletes a service
        /// </summary>
        /// <param name="id">Service ID</param>
        /// <response code="200">Service deleted successfully</response>
        /// <response code="404">Service not found</response>
        [HttpDelete("{id:int:min(1)}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> DeleteService([FromRoute] int id)
        {
            _logger.LogInformation("Deleting service {ServiceId}", id);
            var result = await _serviceService.DeleteAsync(id);

            if (!result)
            {
                _logger.LogWarning("Service {ServiceId} not found for deletion", id);
                return NotFound("Service not found");
            }
            _logger.LogInformation("Service {ServiceId} deleted successfully", id);

            return Ok();
        }



    }
}
