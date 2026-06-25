namespace EgyptianMuseum.Application.DTOs.Rooms
{
    public class CreateRoomRequestDto
    {
        public string Name { get; set; } = null!;
        public string Description { get; set; } = null!;
        public int MapId { get; set; }
        public double XCoord { get; set; }
        public double YCoord { get; set; }
        public List<RoomTranslationRequestDto> Translations { get; set; } = new();
    }
}
