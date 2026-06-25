namespace EgyptianMuseum.Application.DTOs.Map
{
    public class MapTranslationRequestDto
    {
        public string LanguageCode { get; set; } = null!;
        public string Name { get; set; } = null!;
        public string ZoneName { get; set; } = null!;
    }
}
