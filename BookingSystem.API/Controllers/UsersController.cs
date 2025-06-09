using BookingSystem.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Data;
using Wasfaty.Application.DTOs.Users;

namespace BookingSystem.API.Controllers
{
    /// <summary>
    /// API for managing users
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly ILogger<UsersController> _logger;

        public UsersController(
            IUserService userService,
            ILogger<UsersController> logger)
        {
            _userService = userService;
            _logger = logger;
        }


        // GET: api/user
        /// <summary>
        /// Retrieves all users
        /// </summary>
        /// <response code="200">Returns the list of users</response>
        /// <response code="204">No users found</response>
        [HttpGet]
        [ProducesResponseType(typeof(List<UserDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<List<UserDto>>> GetAllUsers()
        {
            _logger.LogInformation("Fetching all users");
            var users = await _userService.GetAllAsync();

            if (!users.Any())
            {
                _logger.LogInformation("No users found");
                return NoContent();
            }
            return Ok(users);
        }

        // GET: api/user/{id}
        /// <summary>
        /// Gets a specific user by ID
        /// </summary>
        /// <param name="id">User ID</param>
        /// <response code="200">Returns the requested user</response>
        /// <response code="404">User not found</response>
        [HttpGet("{id:int:min(1)}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<UserDto>> GetUserById([FromRoute] int id)
        {
            _logger.LogInformation("Fetching user with ID {UserId}", id);

            var user = await _userService.GetByIdAsync(id);
            if (user == null)
            {
                _logger.LogWarning("User with ID {UserId} not found", id);
                return NotFound("User not found");
            }

            return Ok(user);
        }

        /// <summary>
        /// Creates a new user
        /// </summary>
        /// <param name="userDto">User data</param>
        /// <response code="201">User created successfully</response>
        /// <response code="400">Invalid input data</response>
        // POST api/user
        [HttpPost]
        [ProducesResponseType(typeof(UserDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult> CreateUser([FromBody, Required] CreateUserDto userDto)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Invalid model state for user creation");
                return BadRequest(ModelState);
            }
            _logger.LogInformation("Creating new user with email {UserEmail}", userDto.Email);


            var result = await _userService.CreateAsync(userDto);

            if (result == null)
            {
                _logger.LogWarning("Failed to create user - email {UserEmail} may already exist", userDto.Email);
                return BadRequest("Username already exists");
            }
            _logger.LogInformation("User {UserId} created successfully", result.Id);

            return CreatedAtAction(nameof(GetUserById), new { id = result.Id }, result);
                  
        }



        // PUT: api/user/{id}
        /// <summary>
        /// Updates an existing user
        /// </summary>
        /// <param name="id">User ID</param>
        /// <param name="userDto">Updated user data</param>
        /// <response code="200">User updated successfully</response>
        /// <response code="400">Invalid input data</response>
        /// <response code="404">User not found</response>
        [HttpPut("{id:int:min(1)}")]
        [ProducesResponseType(typeof(UserDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<UserDto>> UpdateUser([FromRoute] int id, [FromBody, Required] UpdateUserDto userDto)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Invalid model state for user update");
                return BadRequest(ModelState);
            }
            _logger.LogInformation("Updating user {UserId}", id);

            var result = await _userService.UpdateAsync(id, userDto);
            if (result == null)
            {
                _logger.LogWarning("User {UserId} not found for update", id);
                return NotFound("User not found");
            }
            _logger.LogInformation("User {UserId} updated successfully", id);

            return Ok(result) ;
        }

        // DELETE: api/user/{id}
        /// <summary>
        /// Deletes a user
        /// </summary>
        /// <param name="id">User ID</param>
        /// <response code="200">User deleted successfully</response>
        /// <response code="404">User not found</response>
        [HttpDelete("{id:int:min(1)}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> DeleteUser([FromRoute] int id)
        {
            _logger.LogInformation("Deleting user {UserId}", id);
            var result = await _userService.DeleteAsync(id);

            if (!result)
            {
                _logger.LogWarning("User {UserId} not found for deletion", id);
                return NotFound("User not found");
            }
            _logger.LogInformation("User {UserId} deleted successfully", id);

            return Ok();
        }



    }
}
