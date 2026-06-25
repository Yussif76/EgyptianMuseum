namespace EgyptianMuseum.Domain.Entities
{
    public class Room : BaseEntity
    {
        public string Name { get; set; } = null!;
        public string Description { get; set; } = null!;
        public int MapId { get; set; }
        public Map Map { get; set; } = null!;
        public double XCoord { get; set; }
        public double YCoord { get; set; }
        public ICollection<Pieces> Pieces { get; set; } = new List<Pieces>();
        public ICollection<TourRoom> TourRooms { get; set; } = new List<TourRoom>();
        public ICollection<IndoorMapPath> FromPaths { get; set; } = new List<IndoorMapPath>();
        public ICollection<IndoorMapPath> ToPaths { get; set; } = new List<IndoorMapPath>();
        public ICollection<RoomTranslation> Translations { get; set; } = new List<RoomTranslation>();
    }
}
