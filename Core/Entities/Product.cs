using Core.Enums;

namespace Core.Entities
{
    public class Product : BaseEntity
    {
        public required string Uid { get; set; }
        public string Name { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public Guid CategoryId { get; set; }
        public virtual ProductCategory? Category { get; set; }
        public Guid PackageUnitId { get; set; }
        public virtual PackageUnit? PackageUnit { get; set; }
        public decimal Length { get; set; }
        public decimal Width { get; set; }
        public decimal Height { get; set; }
        public bool Status { get; set; }
        public ProductCategoryType Type { get; set; } = ProductCategoryType.Unknown;
        public ApprovalStatus ApprovalStatus { get; set; }
    }
}