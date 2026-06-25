namespace EgyptianMuseum.Domain.Entities
{
    public class IndoorMapPath : BaseEntity
    {
        public int MapId { get; set; }
        public Map Map { get; set; } = null!;
        public int FromRoomId { get; set; }
        public Room FromRoom { get; set; } = null!;
        public int ToRoomId { get; set; }
        public Room ToRoom { get; set; } = null!;
    }
}
