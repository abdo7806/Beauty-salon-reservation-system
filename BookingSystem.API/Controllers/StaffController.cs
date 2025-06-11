using BookingSystem.Application.DTOs.Staff;
using BookingSystem.Application.Interfaces.Services;
using BookingSystem.Application.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using Wasfaty.Application.DTOs.Users;

namespace BookingSystem.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StaffController : ControllerBase
    {
        private readonly IStaffService _staffService;
        private readonly IUserService _userService;
        private readonly ILogger<StaffController> _logger;

        public StaffController(
            IStaffService staffService,
            IUserService userService,
            ILogger<StaffController> logger)
        {
            _staffService = staffService;
            _userService = userService;
            _logger = logger;
        }


        // GET: api/staff
        /// <summary>
        /// Retrieves all staffs
        /// </summary>
        /// <response code="200">Returns the list of staffs</response>
        /// <response code="204">No staffs found</response>
        [HttpGet]
        [ProducesResponseType(typeof(List<StaffDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<List<StaffDto>>> GetAllStaffs()
        {
            _logger.LogInformation("Fetching all staffs");
            var staffs = await _staffService.GetAllAsync();

            if (!staffs.Any())
            {
                _logger.LogInformation("No staffs found");
                return NoContent();
            }
            return Ok(staffs);
        }

        // GET: api/staff/{id}
        /// <summary>
        /// Gets a specific staff by ID
        /// </summary>
        /// <param name="id">Staff ID</param>
        /// <response code="200">Returns the requested staff</response>
        /// <response code="404">Staff not found</response>
        [HttpGet("{id:int:min(1)}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<StaffDto>> GetStaffById([FromRoute] int id)
        {
            _logger.LogInformation("Fetching staff with ID {StaffId}", id);

            var staff = await _staffService.GetByIdAsync(id);
            if (staff == null)
            {
                _logger.LogWarning("Staff with ID {StaffId} not found", id);
                return NotFound("Staff not found");
            }

            return Ok(staff);
        }

        /// <summary>
        /// Creates a new staff
        /// </summary>
        /// <param name="staffDto">Staff data</param>
        /// <response code="201">Staff created successfully</response>
        /// <response code="400">Invalid input data</response>
        // POST api/staff
        [HttpPost]
        [ProducesResponseType(typeof(StaffDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult> CreateStaff([FromBody, Required] CreateStaffDto staffDto)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Invalid model state for staff creation");
                return BadRequest(ModelState);
            }


            var staffList = await _staffService.GetAllAsync();
            if (staffList.Where(s => s.UserId == staffDto.UserId).Count() > 0)
            {
                return BadRequest("هاذا المستخدم طبيب بالفعل");
            }

            UserDto user = await _userService.GetByIdAsync(staffDto.UserId);
            if (user == null)
            {
                return BadRequest("Invalid User data.");
            }

            if (user.Role != "Staff")
            {
                return BadRequest("لازم تكون صلاحيات المستخدم موظف");

            }

            var result = await _staffService.CreateAsync(staffDto);

            if (result == null)
            {
                return BadRequest("Staffname already exists");
            }
            _logger.LogInformation("Staff {StaffId} created successfully", result.Id);

            return CreatedAtAction(nameof(GetStaffById), new { id = result.Id }, result);

        }



        // PUT: api/staff/{id}
        /// <summary>
        /// Updates an existing staff
        /// </summary>
        /// <param name="id">Staff ID</param>
        /// <param name="staffDto">Updated staff data</param>
        /// <response code="200">Staff updated successfully</response>
        /// <response code="400">Invalid input data</response>
        /// <response code="404">Staff not found</response>
        [HttpPut("{id:int:min(1)}")]
        [ProducesResponseType(typeof(StaffDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<StaffDto>> UpdateStaff([FromRoute] int id, [FromBody, Required] UpdateStaffDto staffDto)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Invalid model state for staff update");
                return BadRequest(ModelState);
            }
            _logger.LogInformation("Updating staff {StaffId}", id);

            var result = await _staffService.UpdateAsync(id, staffDto);
            if (result == null)
            {
                _logger.LogWarning("Staff {StaffId} not found for update", id);
                return NotFound("Staff not found");
            }
            _logger.LogInformation("Staff {StaffId} updated successfully", id);

            return Ok(result);
        }

        // DELETE: api/staff/{id}
        /// <summary>
        /// Deletes a staff
        /// </summary>
        /// <param name="id">Staff ID</param>
        /// <response code="200">Staff deleted successfully</response>
        /// <response code="404">Staff not found</response>
        [HttpDelete("{id:int:min(1)}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> DeleteStaff([FromRoute] int id)
        {
            _logger.LogInformation("Deleting staff {StaffId}", id);
            var result = await _staffService.DeleteAsync(id);

            if (!result)
            {
                _logger.LogWarning("Staff {StaffId} not found for deletion", id);
                return NotFound("Staff not found");
            }
            _logger.LogInformation("Staff {StaffId} deleted successfully", id);

            return Ok();
        }
    }
}
