using EgyptianMuseum.Domain.Entities;

namespace EgyptianMuseum.Application.Interfaces
{
    /// <summary>
    /// Repository interface for navigation-related data access.
    /// </summary>
    public interface INavigationRepository
    {
        /// <summary>
        /// Gets a room by its ID.
        /// </summary>
        /// <param name="roomId">The ID of the room to retrieve.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>The room if found, null otherwise.</returns>
        Task<Room?> GetRoomByIdAsync(int roomId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets all rooms and indoor map paths for a specific map to build the navigation graph.
        /// </summary>
        /// <param name="mapId">The ID of the map.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>A tuple containing all rooms and all paths for the map.</returns>
        Task<(List<Room> Rooms, List<IndoorMapPath> Paths)> GetMapGraphAsync(int mapId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Checks if two rooms belong to the same map.
        /// </summary>
        /// <param name="fromRoomId">The ID of the first room.</param>
        /// <param name="toRoomId">The ID of the second room.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>True if both rooms belong to the same map, false otherwise.</returns>
        Task<bool> RoomsBelongToSameMapAsync(int fromRoomId, int toRoomId, CancellationToken cancellationToken = default);
    }
}
