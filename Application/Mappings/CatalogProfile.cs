using Application.Dtos;
using AutoMapper;
using Core.Entities;

namespace Application.Mappings
{
    public class CatalogProfile : Profile
    {
        public CatalogProfile()
        {
            CreateMap<Warehouse, WarehouseDto>();
            CreateMap<WarehouseDto, Warehouse>();

            CreateMap<WarehouseHistory, WarehouseHistoryDto>();
            CreateMap<WarehouseHistoryDto, WarehouseHistory>();
        }
    }
}