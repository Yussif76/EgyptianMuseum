namespace EgyptianMuseum.Application.DTOs.Tours
{
    public class TourDetailsResponseDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string Description { get; set; } = null!;
        public int DurationMinutes { get; set; }
        public string Category { get; set; } = null!;
        public List<TourRoomResponseDto> Rooms { get; set; } = new List<TourRoomResponseDto>();
    }
}
