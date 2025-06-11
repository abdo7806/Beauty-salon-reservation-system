using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Wasfaty.Application.DTOs.Auth;
using Wasfaty.Application.Interfaces;

namespace BookingSystem.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("register")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [AllowAnonymous]

        public async Task<ActionResult> Register([FromBody] RegisterUserDto request)
        {
            if (request == null || string.IsNullOrEmpty(request.FullName) || string.IsNullOrEmpty(request.Email) || string.IsNullOrEmpty(request.Password))
            {
                return BadRequest("Invalid User data.");
            }
            var user = await _authService.RegisterAsync(request);
            if (user == null)
            {
                return BadRequest("اسم المستخدم موجود بالفعل");
            }
            return Ok(user);
        }

        [HttpPost("login")]
        // [AllowAnonymous]

        public async Task<IActionResult> Login([FromBody] LoginRequestDto request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            var token = await _authService.LoginAsync(request);

            if (token == null)
            {
                return BadRequest("تاكد من كلمة المرور");
            }
            return Ok(token);
        }



        [HttpPost("change-password")]
        public async Task<IActionResult> ChangePasswordAsync([FromBody] ChangePasswordDto model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _authService.ChangeUserPassword(model.UserId, model.CurrentPassword, model.NewPassword);

            if (!result)
                return BadRequest("Failed to change password.");

            return Ok("Password changed successfully.");
        }
    }
}
