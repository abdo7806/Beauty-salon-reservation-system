using BookingSystem.Application.Constants;
using BookingSystem.Application.DTOs.Appointments;
using BookingSystem.Application.Interfaces.Services;
using BookingSystem.Application.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace BookingSystem.API.Controllers
{
    [Route("api/appointments")]
    [ApiController]
    public class AppointmentsController : ControllerBase
    {
        private readonly IAppointmentService _appointmentService;
        private readonly ILogger<AppointmentsController> _logger;

        public AppointmentsController(
            IAppointmentService appointmentService,
            ILogger<AppointmentsController> logger)
        {
            _appointmentService = appointmentService;
            _logger = logger;
        }

        // GET: api/appointments
        /// <summary>
        /// Retrieves all appointments
        /// </summary>
        /// <response code="200">Returns the list of appointments</response>
        /// <response code="204">No appointments found</response>
        [HttpGet]
        [ProducesResponseType(typeof(List<AppointmentDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<List<AppointmentDto>>> GetAll()
        {
            _logger.LogInformation("Fetching all appointments");
            var appointments = await _appointmentService.GetAllAsync();

            if (!appointments.Any())
            {
                _logger.LogInformation("No appointments found");
                return Ok(new List<AppointmentDto>()); // بدلاً من NoContent()
            }
            return Ok(appointments);
        
        }

        // GET: api/appointment/{id}
        /// <summary>
        /// Gets a specific appointment by ID
        /// </summary>
        /// <param name="id">Appointment ID</param>
        /// <response code="200">Returns the requested appointment</response>
        /// <response code="404">Appointment not found</response>
        [HttpGet("{id:int:min(1)}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<AppointmentDto>> GetById(int id)
        {
            _logger.LogInformation("Fetching appointment with ID {AppointmentId}", id);

            var appointment = await _appointmentService.GetByIdAsync(id);
            if (appointment == null)
            {
                _logger.LogWarning("Appointment with ID {AppointmentId} not found", id);
                return NotFound("Appointment not found");
            }

            return Ok(appointment);
        }


        // GET: api/appointments/client/{clientId}
        [HttpGet("client/{clientId:int:min(1)}")]
        public async Task<ActionResult<List<AppointmentDto>>> GetByClientId(int clientId)
        {
            var result = await _appointmentService.GetByClientIdAsync(clientId);
            return result == null ? NotFound() : Ok(result);
        }

        // GET: api/appointments/staff/{staffId}
        [HttpGet("staff/{staffId:int:min(1)}")]
        public async Task<ActionResult<List<AppointmentDto>>> GetByStaffId(int staffId)
        {
            var result = await _appointmentService.GetByStaffIdAsync(staffId);
            return result == null ? NotFound() : Ok(result);
        }

        // POST: api/appointments

        /// <summary>
        /// Creates a new appointment
        /// </summary>
        /// <param name="appointmentDto">Appointment data</param>
        /// <response code="201">Appointment created successfully</response>
        /// <response code="400">Invalid input data</response>
        // POST api/appointment
        [HttpPost("create")]
        [ProducesResponseType(typeof(AppointmentDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult> Create([FromBody, Required] CreateAppointmentDto appointmentDto)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Invalid model state for appointment creation");
                return BadRequest(ModelState);
            }


            var result = await _appointmentService.CreateAsync(appointmentDto);

            if (result == null)
            {
                _logger.LogWarning("Failed to create appointment");
                return BadRequest("Appointment creation failed due to staff unavailability or conflict");
            }
            _logger.LogInformation("Appointment {AppointmentId} created successfully", result.Id);

      
            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        }



        // PUT: api/appointments/{id}/status
        [HttpPut("{id:int:min(1)}/status")]
        public async Task<ActionResult<AppointmentDto>> UpdateStatus(int id, [FromQuery] string status)
        {
  
            if (!AppointmentStatuses.All.Contains(status))
            {
                _logger.LogWarning("Invalid status value: {Status}", status);

                return BadRequest($"Invalid status. Allowed: {string.Join(", ", AppointmentStatuses.All)}");
            }
            var result = await _appointmentService.UpdateStatusAsync(id, status);
            return result == null ? NotFound() : Ok(result);
        }

        // DELETE: api/appointment/{id}
        /// <summary>
        /// Deletes a appointment
        /// </summary>
        /// <param name="id">Appointment ID</param>
        /// <response code="200">Appointment deleted successfully</response>
        /// <response code="404">Appointment not found</response>
        [HttpDelete("{id:int:min(1)}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> DeleteAppointment([FromRoute] int id)
        {
            _logger.LogInformation("Deleting appointment {AppointmentId}", id);
            var result = await _appointmentService.DeleteAsync(id);

            if (!result)
            {
                _logger.LogWarning("Appointment {AppointmentId} not found for deletion", id);
                return NotFound("Appointment not found");
            }
            _logger.LogInformation("Appointment {AppointmentId} deleted successfully", id);

            return Ok();
        }


    }
}
