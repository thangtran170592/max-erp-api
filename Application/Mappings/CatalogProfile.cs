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
            CreateMap<WarehouseHistory, WarehouseHistoryDto>().ReverseMap();
            CreateMap<Warehouse, WarehouseHistoryDto>()
                .ForMember(dest => dest.WarehouseId, opt => opt.Ignore());

            CreateMap<WarehouseRequestDto, Warehouse>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedBy, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedBy, opt => opt.Ignore());
        }
    }
}