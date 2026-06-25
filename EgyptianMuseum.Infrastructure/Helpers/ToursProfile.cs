using EgyptianMuseum.Application.DTOs.Tours;
using EgyptianMuseum.Domain.Entities;
using AutoMapper;

namespace EgyptianMuseum.Infrastructure.Helpers
{
    public class ToursProfile : Profile
    {
        public ToursProfile()
        {
            CreateMap<CreateTourRequestDto, Tour>();
            CreateMap<UpdateTourRequestDto, Tour>();
            CreateMap<TourTranslationDto, TourTranslation>();
            CreateMap<TourMarkDto, TourMarkDto>();
            CreateMap<Tour, TourResponseDto>();
        }
    }
}
