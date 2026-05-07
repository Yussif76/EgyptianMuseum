namespace EgyptianMuseum.Application.DTOs.Map
{
    public class CreateMapRequestDto
    {
        public string Name { get; set; } = null!;
        public string Zone { get; set; } = null!;
        public string ImageUrl { get; set; } = null!;
    }
}
