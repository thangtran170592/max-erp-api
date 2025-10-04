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

            CreateMap<Product, ProductResponseDto>()
                .ForMember(d => d.CategoryName,
                    o => o.MapFrom(s => s.Category != null ? s.Category.Name : string.Empty))
                .ForMember(d => d.PackageUnitId,
                    o => o.MapFrom(s => s.PackageUnit != null && s.PackageUnit.Package != null
                        ? s.PackageUnit.Package.Id
                        : Guid.Empty))
                .ForMember(d => d.PackageUnitName,
                    o => o.MapFrom(s => s.PackageUnit != null && s.PackageUnit.Package != null
                        ? s.PackageUnit.Package.Name
                        : string.Empty))
                .ForMember(d => d.UnitOfMeasureId,
                    o => o.MapFrom(s => s.PackageUnit != null && s.PackageUnit.Unit != null
                        ? s.PackageUnit.Unit.Id
                        : Guid.Empty))
                .ForMember(d => d.UnitOfMeasureName,
                    o => o.MapFrom(s => s.PackageUnit != null && s.PackageUnit.Unit != null
                        ? s.PackageUnit.Unit.Name
                        : string.Empty))
                .ReverseMap()
                .ForMember(d => d.PackageUnit, o => o.Ignore())
                .ForMember(d => d.Category, o => o.Ignore());

            CreateMap<ProductRequestDto, Product>();
            CreateMap<Product, ProductHistory>();
            CreateMap<ProductCategory, ProductCategoryResponseDto>().ReverseMap();
            CreateMap<ProductCategoryRequestDto, ProductCategory>();
            CreateMap<Supplier, SupplierResponseDto>().ReverseMap();
            CreateMap<SupplierRequestDto, Supplier>();
            CreateMap<UnitOfMeasure, UnitOfMeasureResponseDto>().ReverseMap();
            CreateMap<UnitOfMeasureRequestDto, UnitOfMeasure>();
            CreateMap<Package, PackageResponseDto>().ReverseMap();
            CreateMap<PackageRequestDto, Package>();
            CreateMap<PackageUnit, PackageUnitResponseDto>()
                .ForMember(d => d.PackageName,
                    o => o.MapFrom(s => s.Package != null ? s.Package.Name : string.Empty))
                .ForMember(d => d.UnitName,
                    o => o.MapFrom(s => s.Unit != null ? s.Unit.Name : string.Empty))
                .ReverseMap()
                .ForMember(d => d.Package, o => o.Ignore())
                .ForMember(d => d.Unit, o => o.Ignore());

            CreateMap<PackageUnitRequestDto, PackageUnit>();
        }
    }
}
