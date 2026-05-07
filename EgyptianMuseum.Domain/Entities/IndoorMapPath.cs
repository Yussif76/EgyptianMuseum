namespace EgyptianMuseum.Domain.Entities
{
    public class IndoorMapPath : BaseEntity
    {
        public int MapId { get; set; }
        public Map Map { get; set; } = null!;
        public string FromRoom { get; set; } = null!;
        public string ToRoom { get; set; } = null!;
        public double FromX { get; set; }
        public double FromY { get; set; }
        public double ToX { get; set; }
        public double ToY { get; set; }
        public double Distance { get; set; }
    }
}
