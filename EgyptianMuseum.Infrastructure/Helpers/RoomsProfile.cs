using EgyptianMuseum.Application.DTOs.Rooms;
using EgyptianMuseum.Domain.Entities;
using AutoMapper;

namespace EgyptianMuseum.Infrastructure.Helpers
{
    public class RoomsProfile : Profile
    {
        public RoomsProfile()
        {
            CreateMap<CreateRoomRequestDto, Room>();
            CreateMap<UpdateRoomRequestDto, Room>()
                .ForMember(dest => dest.Translations, opt => opt.Ignore());
            CreateMap<RoomTranslationRequestDto, RoomTranslation>();
        }
    }
}
