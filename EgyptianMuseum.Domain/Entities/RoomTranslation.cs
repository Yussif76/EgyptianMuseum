namespace EgyptianMuseum.Domain.Entities
{
    public class RoomTranslation : BaseEntity
    {
        public int RoomId { get; set; }
        public Room Room { get; set; }
        public string LanguageCode { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
    }
}
