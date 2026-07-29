namespace EgyptianMuseum.Application.DTOs.Tours
{
    using EgyptianMuseum.Application.DTOs.Pieces;

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

        public List<TourPieceResponseDto> Pieces { get; set; } = new();
    }

    public class TourPieceResponseDto
    {
        public int Id { get; set; }

        public string Code { get; set; } = null!;

        public List<string> PhotoPaths { get; set; } = new();

        public PieceLocationDto? PieceLocation { get; set; }

        public string Name { get; set; } = null!;

        public string TextNarration { get; set; } = null!;

        public string Period { get; set; } = null!;

        public string Category { get; set; } = null!;
    }
}
