using EgyptianMuseum.Application.DTOs.Auth;
using EgyptianMuseum.Application.Services.Auth;
using Microsoft.AspNetCore.Mvc;

namespace EgyptianMuseum.API.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequestDto dto)
        {
            try
            {
                var userId = await _authService.RegisterAsync(dto);
                return Ok(new { success = true, message = "User registered successfully", userId });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { success = false, errors = new[] { ex.Message } });
            }
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestDto dto)
        {
            try
            {
                var response = await _authService.LoginAsync(dto);
                return Ok(new { success = true, data = response });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { success = false, errors = new[] { ex.Message } });
            }
        }
    }
}
