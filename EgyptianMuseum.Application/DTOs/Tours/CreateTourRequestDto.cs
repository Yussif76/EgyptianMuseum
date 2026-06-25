namespace EgyptianMuseum.Application.DTOs.Tours
{
    public class CreateTourRequestDto
    {
        public string Name { get; set; } = null!;

        public string Description { get; set; } = null!;

        public string Category { get; set; } = null!;

        public int DurationMinutes { get; set; }

        public string Color { get; set; } = null!;

        public string ImageUrl { get; set; } = null!;
        public string IconPath { get; set; } = null!;
        public string PathImageUrl { get; set; } = null!;
        public bool IsRecommended { get; set; }
        public List<string> PieceCodes { get; set; } = new();

        public List<TourMarkDto> Marks { get; set; } = new();

        public List<TourTranslationDto> Translations { get; set; } = new();
    }
}
