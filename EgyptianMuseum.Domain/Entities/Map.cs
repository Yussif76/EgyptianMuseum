namespace EgyptianMuseum.Domain.Entities
{
    public class Map : BaseEntity
    {
        public string Name { get; set; } = null!;
        public string Zone { get; set; } = null!;
        public string ImageUrl { get; set; } = null!;
        public int Height { get; set; }
        public int Width { get; set; }
        public ICollection<IndoorMapPath> Paths { get; set; } = new List<IndoorMapPath>();
        public ICollection<Room> Rooms { get; set; } = new List<Room>();
        public ICollection<MapTranslation> Translations { get; set; } = new List<MapTranslation>();
    }
}
