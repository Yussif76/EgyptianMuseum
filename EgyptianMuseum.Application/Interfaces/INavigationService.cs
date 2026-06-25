using EgyptianMuseum.Application.DTOs.Navigation;

namespace EgyptianMuseum.Application.Interfaces
{
    /// <summary>
    /// Service interface for navigation and pathfinding operations.
    /// Uses Dijkstra's Algorithm to find the shortest path between rooms.
    /// </summary>
    public interface INavigationService
    {
        /// <summary>
        /// Calculates the shortest path between two rooms using Dijkstra's Algorithm.
        /// </summary>
        /// <param name="request">The shortest path request containing source and destination room IDs.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>A response DTO containing the path and total distance, or an error message if no path exists.</returns>
        /// <remarks>
        /// Validates that:
        /// - Both room IDs are valid
        /// - The rooms are different
        /// - Both rooms belong to the same map
        /// - A path exists between the rooms
        /// 
        /// Returns BadRequest status for validation errors.
        /// Returns NotFound if rooms don't exist or no path exists between them.
        /// </remarks>
        Task<ShortestPathResponseDto> GetShortestPathAsync(ShortestPathRequestDto request, CancellationToken cancellationToken = default);

        /// <summary>
        /// Calculates the shortest path between two rooms with language support for room names.
        /// </summary>
        /// <param name="request">The shortest path request containing source and destination room IDs.</param>
        /// <param name="lang">Language code for room name translation (default: "en").</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>A response DTO containing the path with translated room names and total distance, or an error message if no path exists.</returns>
        Task<ShortestPathResponseDto> GetShortestPathAsync(ShortestPathRequestDto request, string lang, CancellationToken cancellationToken = default);
    }
}
