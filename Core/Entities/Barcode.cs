using Core.Enums;

namespace Core.Entities
{
    public class Barcode : BaseEntity
    {
        public BarcodeType BarcodeType { get; set; }
        public BarcodeSize Size { get; set; } = BarcodeSize.Small;
        public BarcodeForType BarcodeForType { get; set; }
        public string Content { get; set; } = string.Empty;
        public string Data { get; set; } = string.Empty;
        public string Metadata { get; set; } = string.Empty;
        public Guid? UserId { get; set; }
    }
}