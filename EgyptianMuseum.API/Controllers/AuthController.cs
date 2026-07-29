using EgyptianMuseum.Application.DTOs.Auth;
using EgyptianMuseum.Application.Services.Auth;
using EgyptianMuseum.Domain.Entities;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace EgyptianMuseum.API.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly UserManager<ApplicationUser> _userManager;

        public AuthController(IAuthService authService, UserManager<ApplicationUser> userManager)
        {
            _authService = authService;
            _userManager = userManager;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequestDto dto)
        {
            try
            {
                var userId = await _authService.RegisterAsync(dto);
                return Ok(new
                {
                    success = true,
                    message = "Account created! Check your email to verify your account.",
                    //requiresEmailConfirmation = true,
                    userId = userId
                });
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
                // Invalid credentials should return 401
                return Unauthorized(new { success = false, errors = new[] { ex.Message } });
            }
        }

        [HttpPost("refresh")]
        public async Task<IActionResult> Refresh(RefreshTokenRequestDto dto)
        {
            try
            {
                var result =
                    await _authService.RefreshTokenAsync(dto);

                return Ok(result);
            }
            catch (Exception ex)
            {
                return Unauthorized(new
                {
                    success = false,
                    errors = new[] { ex.Message }
                });
            }
        }

        //[Authorize]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpGet("me")]
        public async Task<IActionResult> Me()
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return Unauthorized();
            }

            var roles = await _userManager.GetRolesAsync(user);

            return Ok(new
            {
                userId = user.Id,
                email = user.Email,
                name = user.Name,
                language = user.PreferredLanguage
                //roles = roles
            });
        }

        [HttpPost("forgot-password")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordRequestDto request)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(request?.Email))
                    return BadRequest(new { success = false, message = "Email is required" });

                await _authService.ForgotPasswordAsync(request);
                return Ok(new { success = true, message = "If an account with this email exists, an OTP will be sent" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        [HttpPost("verify-otp")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> VerifyOtp([FromBody] VerifyOtpRequestDto request)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(request?.Email) || string.IsNullOrWhiteSpace(request?.Otp))
                    return BadRequest(new { success = false, message = "Email and OTP are required" });

                var isValid = await _authService.VerifyOtpAsync(request);
                if (isValid)
                    return Ok(new { success = true, message = "OTP verified successfully" });

                return BadRequest(new { success = false, message = "Invalid OTP" });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        [HttpPost("reset-password")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequestDto request)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(request?.Email) || 
                    string.IsNullOrWhiteSpace(request?.Otp) ||
                    string.IsNullOrWhiteSpace(request?.NewPassword))
                    return BadRequest(new { success = false, message = "Email, OTP, and new password are required" });

                await _authService.ResetPasswordAsync(request);
                return Ok(new { success = true, message = "Password has been reset successfully" });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        [HttpPost("google-login")]
        public async Task<IActionResult> GoogleLogin([FromBody] GoogleLoginRequestDto dto)
        {
            try
            {
                var response = await _authService.GoogleLoginAsync(dto);
                return Ok(new { success = true, data = response });
            }
            catch (InvalidOperationException ex)
            {
                return Unauthorized(new { success = false, errors = new[] { ex.Message } });
            }
        }

        [HttpGet("confirm-email")]
        public async Task<IActionResult> ConfirmEmail(string userId, string token)
        {
            var result = await _authService.ConfirmEmailAsync(userId, token);

            if (!result)
                return BadRequest("Invalid confirmation link.");

            return Ok("Email confirmed successfully.");
        }
        
        
        
        [HttpPost("resend-confirmation-email")]
        public async Task<IActionResult> ResendConfirmationEmail(
            ResendConfirmationEmailRequestDto dto)
        {
            await _authService.ResendConfirmationEmailAsync(dto.Email);

            return Ok(new
            {
                message = "Verification email has been sent."
            });
        }

        /// <summary>
        /// Updates the display name of the authenticated user.
        /// </summary>
        /// <param name="request">The request containing the new display name.</param>
        /// <returns>
        /// 200 OK if display name changed successfully.
        /// 400 Bad Request if validation fails.
        /// 401 Unauthorized if user is not authenticated.
        /// </returns>
        /// <remarks>
        /// This endpoint updates only the user's display name (Name field).
        /// It does NOT modify username, email, or password.
        /// </remarks>
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpPut("change-username")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> ChangeUserName([FromBody] ChangeUserNameRequestDto request)
        {
            try
            {
                // Get the current user ID from JWT claims
                var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized(new { success = false, message = "User ID not found in token" });
                }

                // Validate request
                if (request == null)
                {
                    return BadRequest(new { success = false, message = "Request cannot be empty" });
                }

                // Call the service
                await _authService.ChangeUserNameAsync(userId, request);

                return Ok(new { success = true, message = "Display name updated successfully" });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = $"An error occurred: {ex.Message}" });
            }
        }
    }
}
