namespace EgyptianMuseum.Application.DTOs.Tours
{
    public class RecommendedTourResponseDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string Description { get; set; } = null!;
        public int DurationMinutes { get; set; }
        public string Category { get; set; } = null!;
        public int RoomsCount { get; set; }
        public int DurationDifference { get; set; }
        public int RoomDifference { get; set; }
        public bool CategoryMatched { get; set; }
    }
}
