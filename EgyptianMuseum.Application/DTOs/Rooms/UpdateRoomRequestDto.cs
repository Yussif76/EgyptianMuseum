namespace EgyptianMuseum.Application.DTOs.Rooms
{
    public class UpdateRoomRequestDto
    {
        public string? Name { get; set; }
        public string? Description { get; set; }
        public int? MapId { get; set; }
        public double? XCoord { get; set; }
        public double? YCoord { get; set; }
        public List<RoomTranslationRequestDto> Translations { get; set; } = new();
    }
}
