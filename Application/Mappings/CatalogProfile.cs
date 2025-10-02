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

            CreateMap<Product, ProductResponseDto>().ReverseMap();
            CreateMap<ProductRequestDto, Product>();
            CreateMap<ProductCategory, ProductCategoryResponseDto>().ReverseMap();
            CreateMap<ProductCategoryRequestDto, ProductCategory>();
            CreateMap<Supplier, SupplierResponseDto>().ReverseMap();
            CreateMap<SupplierRequestDto, Supplier>();
            CreateMap<UnitOfMeasure, UnitOfMeasureResponseDto>().ReverseMap();
            CreateMap<UnitOfMeasureRequestDto, UnitOfMeasure>();
            CreateMap<Package, PackageResponseDto>().ReverseMap();
            CreateMap<PackageRequestDto, Package>();
            CreateMap<PackageUnit, PackageUnitResponseDto>().ReverseMap();
            CreateMap<PackageUnitRequestDto, PackageUnit>();
        }
    }
}
