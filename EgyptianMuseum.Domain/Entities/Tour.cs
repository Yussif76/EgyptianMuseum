namespace EgyptianMuseum.Domain.Entities
{
    public class Tour : BaseEntity
    {
        public string Name { get; set; } = null!;

        public string Description { get; set; } = null!;

        public string Category { get; set; } = null!;

        public int DurationMinutes { get; set; }

        public string Color { get; set; } = null!;

        public string ImageUrl { get; set; } = null!;

        public string PathImageUrl { get; set; } = null!;
        public bool IsRecommended { get; set; } = false;
        public string IconPath { get; set; } = null!;

        // serialized JSON
        public string MarksJson { get; set; } = "[]";

        public ICollection<TourTranslation> Translations { get; set; }
            = new List<TourTranslation>();

        public ICollection<TourPiece> TourPieces { get; set; }
            = new List<TourPiece>();

        // Keep for backward compatibility during migration
        public ICollection<TourRoom> TourRooms { get; set; } = new List<TourRoom>();
    }
}
