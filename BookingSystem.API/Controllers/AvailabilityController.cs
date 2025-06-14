using BookingSystem.Application.DTOs.Availabilities;
using BookingSystem.Application.Interfaces.Services;
using BookingSystem.Application.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace BookingSystem.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AvailabilityController : ControllerBase
    {
        private readonly IAvailabilityService _availabilityService;
        private readonly ILogger<AvailabilityController> _logger;

        public AvailabilityController(
            IAvailabilityService availabilityService,
            ILogger<AvailabilityController> logger)
        {
            _availabilityService = availabilityService;
            _logger = logger;
        }


        // GET: api/availability
        /// <summary>
        /// Retrieves all availabilitys
        /// </summary>
        /// <response code="200">Returns the list of availabilitys</response>
        /// <response code="204">No availabilitys found</response>
        [HttpGet]
        [ProducesResponseType(typeof(List<AvailabilityDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<List<AvailabilityDto>>> GetAllAvailabilitys()
        {
            _logger.LogInformation("Fetching all availabilitys");
            var availabilitys = await _availabilityService.GetAllAsync();

            if (!availabilitys.Any())
            {
                _logger.LogInformation("No availabilitys found");
                return NoContent();
            }
            return Ok(availabilitys);
        }

        // GET: api/availability/{id}
        /// <summary>
        /// Gets a specific availability by ID
        /// </summary>
        /// <param name="id">Availability ID</param>
        /// <response code="200">Returns the requested availability</response>
        /// <response code="404">Availability not found</response>
        [HttpGet("{id:int:min(1)}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<AvailabilityDto>> GetAvailabilityById([FromRoute] int id)
        {
            _logger.LogInformation("Fetching availability with ID {AvailabilityId}", id);

            var availability = await _availabilityService.GetByIdAsync(id);
            if (availability == null)
            {
                _logger.LogWarning("Availability with ID {AvailabilityId} not found", id);
                return NotFound("Availability not found");
            }

            return Ok(availability);
        }

        /// <summary>
        /// Creates a new availability
        /// </summary>
        /// <param name="availabilityDto">Availability data</param>
        /// <response code="201">Availability created successfully</response>
        /// <response code="400">Invalid input data</response>
        // POST api/availability
        [HttpPost]
        [ProducesResponseType(typeof(AvailabilityDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult> CreateAvailability([FromBody, Required] CreateAvailabilityDto availabilityDto)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Invalid model state for availability creation");
                return BadRequest(ModelState);
            }


            var result = await _availabilityService.CreateAsync(availabilityDto);

            if (result == null)
            {
                return BadRequest("Availabilityname already exists");
            }
            _logger.LogInformation("Availability {AvailabilityId} created successfully", result.Id);

            return CreatedAtAction(nameof(GetAvailabilityById), new { id = result.Id }, result);

        }



        // PUT: api/availability/{id}
        /// <summary>
        /// Updates an existing availability
        /// </summary>
        /// <param name="id">Availability ID</param>
        /// <param name="availabilityDto">Updated availability data</param>
        /// <response code="200">Availability updated successfully</response>
        /// <response code="400">Invalid input data</response>
        /// <response code="404">Availability not found</response>
        [HttpPut("{id:int:min(1)}")]
        [ProducesResponseType(typeof(AvailabilityDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<AvailabilityDto>> UpdateAvailability([FromRoute] int id, [FromBody, Required] UpdateAvailabilityDto availabilityDto)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Invalid model state for availability update");
                return BadRequest(ModelState);
            }
            _logger.LogInformation("Updating availability {AvailabilityId}", id);

            var result = await _availabilityService.UpdateAsync(id, availabilityDto);
            if (result == null)
            {
                _logger.LogWarning("Availability {AvailabilityId} not found for update", id);
                return NotFound("Availability not found");
            }
            _logger.LogInformation("Availability {AvailabilityId} updated successfully", id);

            return Ok(result);
        }

        // DELETE: api/availability/{id}
        /// <summary>
        /// Deletes a availability
        /// </summary>
        /// <param name="id">Availability ID</param>
        /// <response code="200">Availability deleted successfully</response>
        /// <response code="404">Availability not found</response>
        [HttpDelete("{id:int:min(1)}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> DeleteAvailability([FromRoute] int id)
        {
            _logger.LogInformation("Deleting availability {AvailabilityId}", id);
            var result = await _availabilityService.DeleteAsync(id);

            if (!result)
            {
                _logger.LogWarning("Availability {AvailabilityId} not found for deletion", id);
                return NotFound("Availability not found");
            }
            _logger.LogInformation("Availability {AvailabilityId} deleted successfully", id);

            return Ok();
        }





        /// <summary>
        /// Gets all availability slots for a specific staff member
        /// </summary>
        /// <param name="staffId">ID of the staff member</param>
        /// <response code="200">Returns list of availability slots</response>
        /// <response code="400">Invalid staff ID</response>
        /// <response code="404">Staff member not found or no availability slots</response>
        [HttpGet("by-staff/{staffId:int:min(1)}")]
        [ProducesResponseType(typeof(List<AvailabilityDto>), 200)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetAvailabilitiesByStaffId([FromRoute] int staffId)
        {
            _logger.LogInformation("Fetching all availability slots for staff {StaffId}", staffId);



            var availabilities = await _availabilityService.GetByStaffIdAsync(staffId);

            if (!availabilities.Any())
            {
                _logger.LogInformation("No availability slots found for staff {StaffId}", staffId);
                return NotFound($"No availability slots found for staff member {staffId}");
            }

            _logger.LogInformation("Retrieved {Count} availability slots for staff {StaffId}",
                availabilities.Count(), staffId);

            return Ok(availabilities);
        }
    }
}
