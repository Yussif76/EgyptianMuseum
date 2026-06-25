namespace EgyptianMuseum.Application.DTOs.Map
{
    public class IndoorMapPathResponseDto
    {
        public int Id { get; set; }
        public int MapId { get; set; }
        public int FromRoomId { get; set; }
        public int ToRoomId { get; set; }
    }
}
