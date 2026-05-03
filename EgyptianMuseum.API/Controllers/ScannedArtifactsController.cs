using EgyptianMuseum.Application.DTOs.ScannedArtifacts;
using EgyptianMuseum.Application.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace EgyptianMuseum.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class ScannedArtifactsController : ControllerBase
    {
        private readonly IScannedArtifactService _scannedArtifactService;

        public ScannedArtifactsController(IScannedArtifactService scannedArtifactService)
        {
            _scannedArtifactService = scannedArtifactService;
        }

        private string GetUserId()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                throw new UnauthorizedAccessException("User not authenticated");
            return userId;
        }

        [HttpPost("scan")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> ScanArtifact(
            [FromBody] ScanArtifactRequestDto request,
            [FromQuery] string lang = "en",
            CancellationToken cancellationToken = default)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(request?.LabelText))
                    return BadRequest(new { success = false, message = "Label text cannot be empty" });

                var userId = GetUserId();
                var result = await _scannedArtifactService.ScanArtifactAsync(userId, request, lang, cancellationToken);
                return StatusCode(StatusCodes.Status201Created, new { success = true, data = result });
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
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GetUserScannedArtifacts(
            [FromQuery] string lang = "en",
            CancellationToken cancellationToken = default)
        {
            try
            {
                var userId = GetUserId();
                var result = await _scannedArtifactService.GetUserScannedArtifactsAsync(userId, cancellationToken, lang);
                return Ok(new { success = true, message = "Scanned artifacts retrieved successfully", data = result });
            }
            catch (UnauthorizedAccessException)
            {
                return Unauthorized(new { success = false, message = "User not authenticated" });
            }
        }

        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById(
            int id,
            [FromQuery] string lang = "en",
            CancellationToken cancellationToken = default)
        {
            try
            {
                if (id <= 0)
                    return BadRequest(new { success = false, message = "Invalid ID" });

                var userId = GetUserId();
                var result = await _scannedArtifactService.GetByIdAsync(userId, id, cancellationToken, lang);
                return Ok(new { success = true, message = "Scanned artifact retrieved successfully", data = result });
            }
            catch (UnauthorizedAccessException)
            {
                return Forbid();
            }
            catch (KeyNotFoundException)
            {
                return NotFound(new { success = false, message = "Scanned artifact not found" });
            }
        }

        [HttpPut("{id}/favorite")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateFavorite(
            int id,
            [FromBody] UpdateScannedArtifactFavoriteRequestDto request,
            CancellationToken cancellationToken)
        {
            try
            {
                if (id <= 0)
                    return BadRequest(new { success = false, message = "Invalid ID" });

                var userId = GetUserId();
                await _scannedArtifactService.UpdateFavoriteAsync(userId, id, request.IsFavorite, cancellationToken);
                return NoContent();
            }
            catch (UnauthorizedAccessException)
            {
                return Forbid();
            }
            catch (KeyNotFoundException)
            {
                return NotFound(new { success = false, message = "Scanned artifact not found" });
            }
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
        {
            try
            {
                if (id <= 0)
                    return BadRequest(new { success = false, message = "Invalid ID" });

                var userId = GetUserId();
                await _scannedArtifactService.DeleteAsync(userId, id, cancellationToken);
                return NoContent();
            }
            catch (UnauthorizedAccessException)
            {
                return Forbid();
            }
            catch (KeyNotFoundException)
            {
                return NotFound(new { success = false, message = "Scanned artifact not found" });
            }
        }

        [HttpPut("pieces/{pieceId}/favorite")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> ToggleFavoriteByPieceId(
            int pieceId,
            [FromBody] UpdateScannedArtifactFavoriteRequestDto request,
            CancellationToken cancellationToken)
        {
            try
            {
                if (pieceId <= 0)
                    return BadRequest(new { success = false, message = "Invalid piece ID" });

                var userId = GetUserId();
                await _scannedArtifactService.UpdateFavoriteByPieceIdAsync(userId, pieceId, request.IsFavorite, cancellationToken);
                return Ok(new { success = true, message = "Favorite status updated successfully" });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { success = false, message = ex.Message });
            }
        }

        [HttpGet("favorites")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GetUserFavorites(
            [FromQuery] string lang = "en",
            CancellationToken cancellationToken = default)
        {
            try
            {
                var userId = GetUserId();
                var result = await _scannedArtifactService.GetUserFavoritesAsync(userId, cancellationToken, lang);
                return Ok(new { success = true, message = "Favorite artifacts retrieved successfully", data = result });
            }
            catch (UnauthorizedAccessException)
            {
                return Unauthorized(new { success = false, message = "User not authenticated" });
            }
        }
    }
}