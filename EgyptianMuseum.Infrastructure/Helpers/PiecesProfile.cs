using EgyptianMuseum.Application.DTOs.Pieces;
using EgyptianMuseum.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;
using AutoMapper;

namespace EgyptianMuseum.Infrastructure.Helpers
{
    public class PiecesProfile : Profile
    {
        public PiecesProfile()
        {

            CreateMap<Pieces, PiecesResponse>()
                .ForMember(dest => dest.Code,
                    opt => opt.MapFrom(src => src.Code))
                .ForMember(dest => dest.Id,
                    opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.PhotoPaths,
                    opt => opt.MapFrom(src => src.Images.Select(img => img.ImagePath).ToList()));

            CreateMap<CreatePiecesRequest, Pieces>()
                .ForMember(dest => dest.Images, opt => opt.Ignore());

            CreateMap<UpdatePiecesRequest, Pieces>()
                .ForMember(dest => dest.Translations, opt => opt.Ignore())
                .ForMember(dest => dest.Images, opt => opt.Ignore());
            CreateMap<PieceTranslationRequest, PieceTranslation>();

        }
    }
    }
