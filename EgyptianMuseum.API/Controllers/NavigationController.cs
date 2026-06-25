using EgyptianMuseum.Application.DTOs.Navigation;
using EgyptianMuseum.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace EgyptianMuseum.API.Controllers
{
    /// <summary>
    /// Controller for indoor navigation endpoints.
    /// Provides pathfinding capabilities for visitors to navigate the museum.
    /// </summary>
    [ApiController]
    [Route("api/navigation")]
    [Produces("application/json")]
    public class NavigationController : ControllerBase
    {
        private readonly INavigationService _navigationService;

        public NavigationController(INavigationService navigationService)
        {
            _navigationService = navigationService;
        }

        /// <summary>
        /// Calculates the shortest path between two rooms using Dijkstra's Algorithm.
        /// </summary>
        /// <param name="request">The request containing source and destination room IDs.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>
        /// A response containing the shortest path with room details and total distance.
        /// </returns>
        /// <remarks>
        /// Example request:
        /// POST /api/navigation/shortest-path
        /// {
        ///   "fromRoomId": 1,
        ///   "toRoomId": 7
        /// }
        /// 
        /// Example response:
        /// {
        ///   "success": true,
        ///   "path": [
        ///     {
        ///       "roomId": 1,
        ///       "name": "Entrance Hall",
        ///       "x": 100,
        ///       "y": 200
        ///     },
        ///     {
        ///       "roomId": 3,
        ///       "name": "Ancient Egypt Hall",
        ///       "x": 250,
        ///       "y": 300
        ///     },
        ///     {
        ///       "roomId": 7,
        ///       "name": "Statue Room",
        ///       "x": 500,
        ///       "y": 600
        ///     }
        ///   ],
        ///   "totalDistance": 120.5,
        ///   "message": null
        /// }
        /// </remarks>
        /// <response code="200">Path found successfully</response>
        /// <response code="400">Validation failed (same room IDs, rooms in different maps, etc.)</response>
        /// <response code="404">Room not found or no path exists</response>
        [HttpPost("shortest-path")]
        [ProducesResponseType(typeof(ShortestPathResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ShortestPathResponseDto), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ShortestPathResponseDto), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetShortestPath([FromBody] ShortestPathRequestDto request, [FromQuery] string lang = "en", CancellationToken cancellationToken = default)
        {
            try
            {
                // Validate request
                if (request == null)
                {
                    return BadRequest(new ShortestPathResponseDto
                    {
                        Success = false,
                        Message = "Request cannot be empty.",
                        Path = new(),
                        TotalDistance = 0
                    });
                }

                if (request.FromRoomId <= 0 || request.ToRoomId <= 0)
                {
                    return BadRequest(new ShortestPathResponseDto
                    {
                        Success = false,
                        Message = "Room IDs must be greater than 0.",
                        Path = new(),
                        TotalDistance = 0
                    });
                }

                // Call service with language parameter
                var result = await _navigationService.GetShortestPathAsync(request, lang, cancellationToken);

                // Return appropriate status code based on result
                if (!result.Success)
                {
                    // Determine if it's a 404 or 400 based on message
                    if (result.Message?.Contains("not found") ?? false)
                    {
                        return NotFound(result);
                    }
                    else if (result.Message?.Contains("No path exists") ?? false)
                    {
                        return NotFound(result);
                    }
                    else
                    {
                        return BadRequest(result);
                    }
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new ShortestPathResponseDto
                {
                    Success = false,
                    Message = $"An error occurred while calculating the path: {ex.Message}",
                    Path = new(),
                    TotalDistance = 0
                });
            }
        }
    }
}
