namespace EgyptianMuseum.Application.DTOs.Navigation
{
    /// <summary>
    /// Represents a room step in the navigation path.
    /// </summary>
    public class NavigationRoomStepDto
    {
        /// <summary>
        /// The unique identifier of the room.
        /// </summary>
        public int RoomId { get; set; }

        /// <summary>
        /// The name of the room.
        /// </summary>
        public string Name { get; set; } = null!;

        /// <summary>
        /// The X coordinate of the room on the map.
        /// </summary>
        public double X { get; set; }

        /// <summary>
        /// The Y coordinate of the room on the map.
        /// </summary>
        public double Y { get; set; }
    }
}
