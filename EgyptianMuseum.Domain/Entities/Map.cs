namespace EgyptianMuseum.Domain.Entities
{
    public class Map : BaseEntity
    {
        public string Name { get; set; } = null!;
        public string Zone { get; set; } = null!;
        public string ImageUrl { get; set; } = null!;
        public ICollection<IndoorMapPath> Paths { get; set; } = new List<IndoorMapPath>();
    }
}
