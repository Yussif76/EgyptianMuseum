namespace EgyptianMuseum.Application.DTOs.Navigation
{
    /// <summary>
    /// Request DTO for finding the shortest path between two rooms.
    /// </summary>
    public class ShortestPathRequestDto
    {
        /// <summary>
        /// The ID of the starting room.
        /// </summary>
        public int FromRoomId { get; set; }

        /// <summary>
        /// The ID of the destination room.
        /// </summary>
        public int ToRoomId { get; set; }
    }
}
