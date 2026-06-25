namespace EgyptianMuseum.Domain.Entities
{
    public class MapTranslation : BaseEntity
    {
        public int MapId { get; set; }
        public Map Map { get; set; }
        public string LanguageCode { get; set; }
        public string Name { get; set; }
        public string ZoneName { get; set; }
    }
}
