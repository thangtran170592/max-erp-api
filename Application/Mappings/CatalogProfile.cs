using Application.Dtos;
using AutoMapper;
using Core.Entities;

namespace Application.Mappings
{
    public class CatalogProfile : Profile
    {
        public CatalogProfile()
        {
            CreateMap<Warehouse, WarehouseResponseDto>();
            CreateMap<WarehouseRequestDto, Warehouse>();

            CreateMap<WarehouseHistory, WarehouseHistoryDto>();
            CreateMap<WarehouseHistoryDto, WarehouseHistory>();
        }
    }
}