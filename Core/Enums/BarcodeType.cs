namespace Core.Enums
{
    public enum BarcodeType
    {
        QrCode = 1,
        Code128 = 2,
        Code39 = 3,
        Ean13 = 4,
        Ean8 = 5,
        Pdf417 = 6,
        DataMatrix = 7,
    }

    public static class BarcodeTypeExtensions
    {
        public static string GetTitle(this BarcodeType type) => type switch
        {
            BarcodeType.QrCode => "QR Code",
            BarcodeType.Code128 => "Code 128",
            BarcodeType.Code39 => "Code 39",
            BarcodeType.Ean13 => "EAN-13",
            BarcodeType.Ean8 => "EAN-8",
            BarcodeType.Pdf417 => "PDF417",
            BarcodeType.DataMatrix => "Data Matrix",
            _ => "Unknown"
        };
    }
}