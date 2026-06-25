using EgyptianMuseum.Application.DTOs.Tours;
using EgyptianMuseum.Application.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EgyptianMuseum.API.Controllers
{
    [ApiController]
    [Route("api/tours")]
    public class ToursController : ControllerBase
    {
        private readonly ITourService _tourService;
        private readonly ILogger<ToursController> _logger;

        public ToursController(ITourService tourService, ILogger<ToursController> logger)
        {
            _tourService = tourService;
            _logger = logger;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetAllTours([FromQuery] string lang = "en", CancellationToken cancellationToken = default)
        {
            try
            {
                _logger.LogInformation("Fetching all tours with language: {Lang}", lang);
                var tours = await _tourService.GetAllAsync(lang, cancellationToken);
                return Ok(new { success = true, data = tours, count = tours.Count });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching tours");
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new { success = false, message = "Error fetching tours" });
            }
        }

        [HttpGet("recommend")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> RecommendTours(
            [FromQuery] string? category,
            [FromQuery] int? durationMinutes,
            [FromQuery] int? numberOfRooms,
            [FromQuery] string lang = "en",
            CancellationToken cancellationToken = default)
        {
            try
            {
                // Validate that at least one parameter is provided
                if (string.IsNullOrWhiteSpace(category) && !durationMinutes.HasValue && !numberOfRooms.HasValue)
                    return BadRequest(new { success = false, message = "At least one filter parameter is required: category, durationMinutes, or numberOfRooms" });

                // Validate provided values
                if (durationMinutes.HasValue && durationMinutes <= 0)
                    return BadRequest(new { success = false, message = "Duration must be greater than 0" });

                if (numberOfRooms.HasValue && numberOfRooms <= 0)
                    return BadRequest(new { success = false, message = "Number of rooms must be greater than 0" });

                _logger.LogInformation("Recommending tours for category: {Category}, duration: {Duration}, rooms: {Rooms}, language: {Lang}",
                    category ?? "not specified", durationMinutes?.ToString() ?? "not specified", numberOfRooms?.ToString() ?? "not specified", lang);

                var recommendedTours = await _tourService.RecommendToursAsync(category, durationMinutes, numberOfRooms, lang, cancellationToken);
                return Ok(new { success = true, data = recommendedTours, count = recommendedTours.Count });
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning("Invalid argument: {Message}", ex.Message);
                return BadRequest(new { success = false, message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error recommending tours");
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new { success = false, message = "Error recommending tours" });
            }
        }
        [HttpGet("recommended")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetRecommendedTours(
    [FromQuery] string lang = "en",
    CancellationToken cancellationToken = default)
        {
            try
            {
                _logger.LogInformation("Fetching recommended tours with language: {Lang}", lang);

                var tours = await _tourService.GetRecommendedAsync(lang, cancellationToken);

                return Ok(new
                {
                    success = true,
                    data = tours,
                    count = tours.Count
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching recommended tours");

                return StatusCode(
                    StatusCodes.Status500InternalServerError,
                    new
                    {
                        success = false,
                        message = "Error fetching recommended tours"
                    });
            }
        }
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetTourById(int id, [FromQuery] string lang = "en", CancellationToken cancellationToken = default)
        {
            try
            {
                if (id <= 0)
                    return BadRequest(new { success = false, message = "Invalid tour ID" });

                _logger.LogInformation("Fetching tour with ID: {TourId}, Language: {Lang}", id, lang);
                var tour = await _tourService.GetByIdAsync(id, lang, cancellationToken);
                return Ok(new { success = true, data = tour });
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning("Tour not found: {TourId}", id);
                return NotFound(new { success = false, message = ex.Message });
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning("Invalid argument: {Message}", ex.Message);
                return BadRequest(new { success = false, message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching tour with ID: {TourId}", id);
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new { success = false, message = "Error fetching tour" });
            }
        }
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> CreateTour(CreateTourRequestDto request, CancellationToken cancellationToken)
        {
            try
            {
                if (request == null)
                    return BadRequest(new { success = false, message = "Request cannot be null" });

                _logger.LogInformation("Creating tour: {TourName}", request.Name);
                var tour = await _tourService.CreateAsync(request, cancellationToken);
                return CreatedAtAction(nameof(GetTourById), new { id = tour.Id },
                    new { success = true, data = tour });
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning("Invalid argument: {Message}", ex.Message);
                return BadRequest(new { success = false, message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating tour");
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new { success = false, message = "Error creating tour" });
            }
        }
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UpdateTour(int id, UpdateTourRequestDto request, CancellationToken cancellationToken)
        {
            try
            {
                if (id <= 0)
                    return BadRequest(new { success = false, message = "Invalid tour ID" });

                if (request == null)
                    return BadRequest(new { success = false, message = "Request cannot be null" });

                _logger.LogInformation("Updating tour with ID: {TourId}", id);
                var tour = await _tourService.UpdateAsync(id, request, cancellationToken);
                return Ok(new { success = true, data = tour });
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning("Tour not found: {TourId}", id);
                return NotFound(new { success = false, message = ex.Message });
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning("Invalid argument: {Message}", ex.Message);
                return BadRequest(new { success = false, message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating tour with ID: {TourId}", id);
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new { success = false, message = "Error updating tour" });
            }
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DeleteTour(int id, CancellationToken cancellationToken)
        {
            try
            {
                if (id <= 0)
                    return BadRequest(new { success = false, message = "Invalid tour ID" });

                _logger.LogInformation("Deleting tour with ID: {TourId}", id);
                var result = await _tourService.DeleteAsync(id, cancellationToken);

                if (!result)
                    return NotFound(new { success = false, message = $"Tour with ID {id} not found" });

                return Ok(new { success = true, message = "Tour deleted successfully" });
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning("Invalid argument: {Message}", ex.Message);
                return BadRequest(new { success = false, message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting tour with ID: {TourId}", id);
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new { success = false, message = "Error deleting tour" });
            }
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpPost("{tourId}/rooms")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> AddRoomToTour(int tourId, AddRoomToTourRequestDto request, CancellationToken cancellationToken)
        {
            try
            {
                if (tourId <= 0)
                    return BadRequest(new { success = false, message = "Invalid tour ID" });

                if (request == null)
                    return BadRequest(new { success = false, message = "Request cannot be null" });

                _logger.LogInformation("Adding room {RoomId} to tour {TourId}", request.RoomId, tourId);
                var tourRoom = await _tourService.AddRoomToTourAsync(tourId, request, cancellationToken);
                return CreatedAtAction(nameof(GetTourRooms), new { tourId },
                    new { success = true, data = tourRoom });
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning("Resource not found: {Message}", ex.Message);
                return NotFound(new { success = false, message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning("Invalid operation: {Message}", ex.Message);
                return Conflict(new { success = false, message = ex.Message });
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning("Invalid argument: {Message}", ex.Message);
                return BadRequest(new { success = false, message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding room to tour");
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new { success = false, message = "Error adding room to tour" });
            }
        }

        [HttpGet("{tourId}/rooms")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetTourRooms(int tourId, CancellationToken cancellationToken)
        {
            try
            {
                if (tourId <= 0)
                    return BadRequest(new { success = false, message = "Invalid tour ID" });

                _logger.LogInformation("Fetching rooms for tour {TourId}", tourId);
                var rooms = await _tourService.GetTourRoomsAsync(tourId, cancellationToken);
                return Ok(new { success = true, data = rooms, count = rooms.Count });
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning("Tour not found: {Message}", ex.Message);
                return NotFound(new { success = false, message = ex.Message });
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning("Invalid argument: {Message}", ex.Message);
                return BadRequest(new { success = false, message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching tour rooms");
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new { success = false, message = "Error fetching tour rooms" });
            }
        }

        [HttpGet("{tourId}/details")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetTourDetails(int tourId, [FromQuery] string lang = "en", CancellationToken cancellationToken = default)
        {
            try
            {
                if (tourId <= 0)
                    return BadRequest(new { success = false, message = "Invalid tour ID" });

                _logger.LogInformation("Fetching details for tour {TourId}, Language: {Lang}", tourId, lang);
                var tourDetails = await _tourService.GetTourDetailsAsync(tourId, lang, cancellationToken);
                return Ok(new { success = true, data = tourDetails });
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning("Tour not found: {Message}", ex.Message);
                return NotFound(new { success = false, message = ex.Message });
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning("Invalid argument: {Message}", ex.Message);
                return BadRequest(new { success = false, message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching tour details");
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new { success = false, message = "Error fetching tour details" });
            }
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpDelete("{tourId}/rooms/{roomId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DeleteRoomFromTour(int tourId, int roomId, CancellationToken cancellationToken)
        {
            try
            {
                if (tourId <= 0 || roomId <= 0)
                    return BadRequest(new { success = false, message = "Invalid tour ID or room ID" });

                _logger.LogInformation("Removing room {RoomId} from tour {TourId}", roomId, tourId);
                var result = await _tourService.DeleteRoomFromTourAsync(tourId, roomId, cancellationToken);

                if (!result)
                    return NotFound(new { success = false, message = $"Room not found in tour" });

                return Ok(new { success = true, message = "Room removed from tour successfully" });
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning("Invalid argument: {Message}", ex.Message);
                return BadRequest(new { success = false, message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error removing room from tour");
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new { success = false, message = "Error removing room from tour" });
            }
        }
    }
}
