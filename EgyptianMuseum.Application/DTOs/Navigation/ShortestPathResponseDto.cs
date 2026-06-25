namespace EgyptianMuseum.Application.DTOs.Navigation
{
    /// <summary>
    /// Response DTO for the shortest path calculation.
    /// </summary>
    public class ShortestPathResponseDto
    {
        /// <summary>
        /// Indicates whether the path was found successfully.
        /// </summary>
        public bool Success { get; set; }

        /// <summary>
        /// The ordered list of rooms representing the shortest path.
        /// </summary>
        public List<NavigationRoomStepDto> Path { get; set; } = new();

        /// <summary>
        /// The total distance of the calculated path.
        /// </summary>
        public double TotalDistance { get; set; }

        /// <summary>
        /// Error message if the path could not be found.
        /// </summary>
        public string? Message { get; set; }
    }
}
