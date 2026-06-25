namespace EgyptianMuseum.Domain.Entities
{
    public class TourRoom
    {
        public int TourId { get; set; }
        public Tour Tour { get; set; } = null!;
        public int RoomId { get; set; }
        public Room Room { get; set; } = null!;
        public int Order { get; set; }
    }
}
