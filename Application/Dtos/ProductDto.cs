using Core.Enums;

namespace Application.Dtos
{
    public record ProductResponseDto : BaseDto
    {
        public string Uid { get; init; } = string.Empty;
        public string Name { get; init; } = string.Empty;
        public decimal Price { get; init; }
        public bool Status { get; init; }
        public Guid CategoryId { get; init; }
        public string CategoryName { get; init; } = string.Empty;
        public Guid PackageUnitId { get; init; }
        public Guid PackageId { get; init; }
        public string? PackageName { get; init; } = string.Empty;
        public Guid UnitOfMeasureId { get; init; }
        public string? UnitOfMeasureName { get; init; } = string.Empty;
        public decimal Length { get; init; }
        public decimal Width { get; init; }
        public decimal Height { get; init; }
        public ProductCategoryType Type { get; set; } = ProductCategoryType.Unknown;
        public string TypeName => Type.GetTitle();
        public ApprovalStatus ApprovalStatus { get; init; }
        public string? ApprovalStatusName => ApprovalStatus.GetTitle();
    }

    public record ProductRequestDto : BaseDto
    {
        public string Uid { get; init; } = string.Empty;
        public string Name { get; init; } = string.Empty;
        public decimal Price { get; init; }
        public Guid CategoryId { get; init; }
        public Guid PackageUnitId { get; init; }
        public bool Status { get; init; } = true;
        public decimal Length { get; init; }
        public decimal Width { get; init; }
        public decimal Height { get; init; }
        public ProductCategoryType Type { get; init; }
    }


    public record UpdateProductStatusRequestDto : BaseDto
    {
        public ApprovalStatus ApprovalStatus { get; init; }
        public string ReasonRejection { get; init; } = string.Empty;
    }
}
