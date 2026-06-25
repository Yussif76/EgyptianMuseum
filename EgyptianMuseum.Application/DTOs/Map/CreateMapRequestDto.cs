namespace EgyptianMuseum.Application.DTOs.Map
{
    public class CreateMapRequestDto
    {
        public string Name { get; set; } = null!;
        public string Zone { get; set; } = null!;
        public string ImageUrl { get; set; } = null!;
        public int Height { get; set; }
        public int Width { get; set; }
        public List<MapTranslationRequestDto> Translations { get; set; } = new();
    }
}
