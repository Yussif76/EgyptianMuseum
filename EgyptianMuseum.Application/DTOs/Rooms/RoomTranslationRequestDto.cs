namespace EgyptianMuseum.Application.DTOs.Rooms
{
    public class RoomTranslationRequestDto
    {
        public string LanguageCode { get; set; } = null!;
        public string Name { get; set; } = null!;
        public string Description { get; set; } = null!;
    }
}
