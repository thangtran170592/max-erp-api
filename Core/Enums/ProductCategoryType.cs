namespace Core.Enums
{
    public enum ProductCategoryType
    {
        Unknown = 1,
        Bom = 2,
        MinMax = 3,
    }

    public static class ProductCategoryTypeExtensions
    {
        public static string GetTitle(this ProductCategoryType type) => type switch
        {
            ProductCategoryType.Bom => "Bom",
            ProductCategoryType.MinMax => "Min-Max",
            _ => "Unknown"
        };
    }
}