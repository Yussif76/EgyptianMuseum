namespace EgyptianMuseum.Application.DTOs.Tours
{
    public class TourRoomResponseDto
    {
        public int TourId { get; set; }
        public int RoomId { get; set; }
        public string RoomName { get; set; } = null!;
        public string RoomDescription { get; set; } = null!;
        public int MapId { get; set; }
        public double XCoord { get; set; }
        public double YCoord { get; set; }
        public int Order { get; set; }
    }
}
