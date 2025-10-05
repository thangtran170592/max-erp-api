namespace Core.Enums
{
    public enum BarcodeSize
    {
        Tiny = 1,
        Small = 2,
        Medium = 3,
        Large = 4,
    }

    public static class BarcodeSizeExtensions
    {
        public static string GetTitle(this BarcodeSize size) => size switch
        {
            BarcodeSize.Tiny => "Tiny",
            BarcodeSize.Small => "Small",
            BarcodeSize.Medium => "Medium",
            BarcodeSize.Large => "Large",
            _ => "Medium"
        };
    }
}