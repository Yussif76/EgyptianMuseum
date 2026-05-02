using EgyptianMuseum.Application.DTOs.Feedback;
using EgyptianMuseum.Application.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace EgyptianMuseum.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FeedbackController : ControllerBase
    {
        private readonly IFeedbackService _feedbackService;

        public FeedbackController(IFeedbackService feedbackService)
        {
            _feedbackService = feedbackService;
        }

        private string GetUserId()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                throw new UnauthorizedAccessException("User not authenticated");
            return userId;
        }

        [HttpPost]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> CreateFeedback(
            [FromBody] CreateFeedbackRequestDto request,
            CancellationToken cancellationToken)
        {
            try
            {
                var userId = GetUserId();
                var result = await _feedbackService.CreateAsync(userId, request, cancellationToken);
                return StatusCode(StatusCodes.Status201Created,
                    new { success = true, message = "Feedback created successfully", data = result });
            }
            catch (UnauthorizedAccessException)
            {
                return Unauthorized(new { success = false, message = "User not authenticated" });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { success = false, message = ex.Message });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetUserFeedback(CancellationToken cancellationToken)
        {
            try
            {
                // Optional user context - if authenticated, return user's own feedback, else return all
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var result = await _feedbackService.GetUserFeedbackAsync(userId, cancellationToken);
                return Ok(new { success = true, message = "User feedback retrieved successfully", data = result });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        [HttpGet("target/{targetType}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetByTarget(
            string targetType,
            [FromQuery] int? targetId,
            CancellationToken cancellationToken)
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(targetType))
                {
                    var normalizedType = targetType.ToLower();
                    if (normalizedType != "app" && (!targetId.HasValue || targetId <= 0))
                    {
                        return BadRequest(new { success = false, message = "Target ID must be provided and greater than 0 for Artifact and Chat feedback" });
                    }
                }

                // GetByTarget is public, so optional user context
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var result = await _feedbackService.GetByTargetAsync(userId, targetType, targetId, cancellationToken);
                return Ok(new { success = true, message = "Target feedback retrieved successfully", data = result });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteFeedback(int id, CancellationToken cancellationToken)
        {
            try
            {
                if (id <= 0)
                    return BadRequest(new { success = false, message = "Invalid feedback ID" });

                var userId = GetUserId();
                await _feedbackService.DeleteAsync(userId, id, cancellationToken);
                return NoContent();
            }
            catch (UnauthorizedAccessException)
            {
                return Forbid();
            }
            catch (KeyNotFoundException)
            {
                return NotFound(new { success = false, message = "Feedback not found" });
            }
        }
    }
}
