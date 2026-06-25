namespace EgyptianMuseum.Application.DTOs.Tours
{
    public class TourResponseDto
    {
        public int Id { get; set; }

        public string Name { get; set; } = null!;

        public string Description { get; set; } = null!;

        public int DurationMinutes { get; set; }

        public string Category { get; set; } = null!;

        public string Color { get; set; } = null!;

        public string ImageUrl { get; set; } = null!;
        public string IconPath { get; set; } = null!;
        public string PathImageUrl { get; set; } = null!;
        public bool IsRecommended { get; set; }
        public List<TourMarkDto> Marks { get; set; } = new();

        public List<TourPieceResponseDto> Pieces { get; set; } = new();
    }

    public class TourPieceResponseDto
    {
        public int Id { get; set; }

        public string Code { get; set; } = null!;

        public string PhotoPath { get; set; } = null!;

        public string Name { get; set; } = null!;

        public string TextNarration { get; set; } = null!;

        public string Period { get; set; } = null!;

        public string Category { get; set; } = null!;
    }
}
