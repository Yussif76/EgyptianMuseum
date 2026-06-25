using EgyptianMuseum.Application.DTOs.Map;
using EgyptianMuseum.Domain.Entities;
using AutoMapper;

namespace EgyptianMuseum.Infrastructure.Helpers
{
    public class MapProfile : Profile
    {
        public MapProfile()
        {
            CreateMap<CreateMapRequestDto, Map>();
            CreateMap<UpdateMapRequestDto, Map>()
                .ForMember(dest => dest.Translations, opt => opt.Ignore());
            CreateMap<MapTranslationRequestDto, MapTranslation>();
        }
    }
}
