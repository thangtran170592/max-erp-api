using Application.Dtos;
using AutoMapper;
using Core.Entities;

namespace Application.Mappings
{
    public class CatalogProfile : Profile
    {
        public CatalogProfile()
        {
            CreateMap<Warehouse, WarehouseResponseDto>().ReverseMap();

            CreateMap<WarehouseHistory, WarehouseHistoryDto>()
                .ForMember(dest => dest.CreatedByUser, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedByUser, opt => opt.Ignore());

            CreateMap<Warehouse, WarehouseHistoryDto>()
                .ForMember(dest => dest.WarehouseId, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedByUser, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedByUser, opt => opt.Ignore());

            CreateMap<Warehouse, WarehouseHistory>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.WarehouseId, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Warehouse, opt => opt.Ignore())
                .ForMember(dest => dest.ConcurrencyStamp, opt => opt.Ignore());

            CreateMap<WarehouseRequestDto, Warehouse>()
                .ForMember(dest => dest.Id, opt => opt.Ignore());
        }
    }
}