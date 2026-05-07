namespace EgyptianMuseum.Application.DTOs.Map
{
    public class IndoorMapPathResponseDto
    {
        public int Id { get; set; }
        public int MapId { get; set; }
        public string FromRoom { get; set; } = null!;
        public string ToRoom { get; set; } = null!;
        public double FromX { get; set; }
        public double FromY { get; set; }
        public double ToX { get; set; }
        public double ToY { get; set; }
        public double Distance { get; set; }
    }
}
