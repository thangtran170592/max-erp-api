using Core.Enums;

namespace Core.Entities
{
    public class ProductCategory : BaseEntity
    {
        public required string Uid { get; set; }
        public string Name { get; set; } = string.Empty;
        public ProductCategoryType Type { get; set; } = ProductCategoryType.Unknown;
        public bool Status { get; set; }
    }
}