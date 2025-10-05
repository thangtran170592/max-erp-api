namespace Core.Enums
{
    public enum BarcodeForType
    {
        Warehouse = 1,
        Supplier = 2,
        Product = 3,
    }

    public static class BarcodeForTypeExtensions
    {
        public static string GetTitle(this BarcodeForType type) => type switch
        {
            BarcodeForType.Warehouse => "Warehouse",
            BarcodeForType.Supplier => "Supplier",
            BarcodeForType.Product => "Product",
            _ => "Unknown"
        };
    }
}