namespace EgyptianMuseum.Application.DTOs.Map
{
    public class CreateIndoorMapPathRequestDto
    {
        public int MapId { get; set; }
        public int FromRoomId { get; set; }
        public int ToRoomId { get; set; }
    }
}
