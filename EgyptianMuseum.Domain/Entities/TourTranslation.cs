namespace EgyptianMuseum.Domain.Entities
{
    public class TourTranslation : BaseEntity
    {
        public int TourId { get; set; }

        public Tour Tour { get; set; } = null!;

        public string LanguageCode { get; set; } = null!;

        public string Name { get; set; } = null!;

        public string Description { get; set; } = null!;

        public string Category { get; set; } = null!;
    }
}
