using EgyptianMuseum.Application.DTOs.Rooms;
using EgyptianMuseum.Application.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EgyptianMuseum.API.Controllers
{
    [ApiController]
    [Route("api/rooms")]
    public class RoomsController : ControllerBase
    {
        private readonly IRoomService _roomService;
        private readonly ILogger<RoomsController> _logger;

        public RoomsController(IRoomService roomService, ILogger<RoomsController> logger)
        {
            _roomService = roomService;
            _logger = logger;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetAllRooms([FromQuery] string lang = "en", CancellationToken cancellationToken = default)
        {
            try
            {
                _logger.LogInformation("Fetching all rooms with language: {Language}", lang);
                var rooms = await _roomService.GetAllAsync(lang, cancellationToken);
                return Ok(new { success = true, data = rooms, count = rooms.Count });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching rooms");
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new { success = false, message = "Error fetching rooms" });
            }
        }

        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetRoomById(int id, [FromQuery] string lang = "en", CancellationToken cancellationToken = default)
        {
            try
            {
                if (id <= 0)
                    return BadRequest(new { success = false, message = "Invalid room ID" });

                _logger.LogInformation("Fetching room with ID: {RoomId} and language: {Language}", id, lang);
                var room = await _roomService.GetByIdAsync(id, lang, cancellationToken);
                return Ok(new { success = true, data = room });
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning("Room not found: {RoomId}", id);
                return NotFound(new { success = false, message = ex.Message });
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning("Invalid argument: {Message}", ex.Message);
                return BadRequest(new { success = false, message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching room with ID: {RoomId}", id);
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new { success = false, message = "Error fetching room" });
            }
        }

        [HttpGet("map/{mapId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetRoomsByMapId(int mapId, [FromQuery] string lang = "en", CancellationToken cancellationToken = default)
        {
            try
            {
                if (mapId <= 0)
                    return BadRequest(new { success = false, message = "Invalid map ID" });

                _logger.LogInformation("Fetching rooms for map ID: {MapId} with language: {Language}", mapId, lang);
                var rooms = await _roomService.GetByMapIdAsync(mapId, lang, cancellationToken);
                return Ok(new { success = true, data = rooms, count = rooms.Count });
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning("Invalid argument: {Message}", ex.Message);
                return BadRequest(new { success = false, message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching rooms for map ID: {MapId}", mapId);
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new { success = false, message = "Error fetching rooms" });
            }
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> CreateRoom(
            [FromBody] CreateRoomRequestDto request,
            CancellationToken cancellationToken)
        {
            try
            {
                if (request == null)
                    return BadRequest(new { success = false, message = "Request body is required" });

                _logger.LogInformation("Creating room: {RoomName}", request.Name);
                var room = await _roomService.CreateAsync(request, cancellationToken);
                return CreatedAtAction(nameof(GetRoomById), new { id = room.Id },
                    new { success = true, data = room });
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning("Invalid argument: {Message}", ex.Message);
                return BadRequest(new { success = false, message = ex.Message });
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning("Resource not found: {Message}", ex.Message);
                return NotFound(new { success = false, message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating room");
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new { success = false, message = "Error creating room" });
            }
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UpdateRoom(
            int id,
            [FromBody] UpdateRoomRequestDto request,
            CancellationToken cancellationToken)
        {
            try
            {
                if (id <= 0)
                    return BadRequest(new { success = false, message = "Invalid room ID" });

                if (request == null)
                    return BadRequest(new { success = false, message = "Request body is required" });

                _logger.LogInformation("Updating room with ID: {RoomId}", id);
                var room = await _roomService.UpdateAsync(id, request, cancellationToken);
                return Ok(new { success = true, data = room });
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning("Invalid argument: {Message}", ex.Message);
                return BadRequest(new { success = false, message = ex.Message });
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning("Resource not found: {Message}", ex.Message);
                return NotFound(new { success = false, message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogError(ex, "Error updating room");
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new { success = false, message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating room with ID: {RoomId}", id);
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new { success = false, message = "Error updating room" });
            }
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DeleteRoom(int id, CancellationToken cancellationToken)
        {
            try
            {
                if (id <= 0)
                    return BadRequest(new { success = false, message = "Invalid room ID" });

                _logger.LogInformation("Deleting room with ID: {RoomId}", id);
                var deleted = await _roomService.DeleteAsync(id, cancellationToken);

                if (!deleted)
                    return NotFound(new { success = false, message = $"Room with ID {id} not found" });

                return NoContent();
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning("Invalid argument: {Message}", ex.Message);
                return BadRequest(new { success = false, message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting room with ID: {RoomId}", id);
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new { success = false, message = "Error deleting room" });
            }
        }
    }
}
