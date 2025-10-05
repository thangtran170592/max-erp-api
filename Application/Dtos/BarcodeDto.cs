using Core.Enums;

namespace Application.Dtos
{
    public record BarcodeRequestDto : BaseDto
    {
        public string Name { get; set; } = string.Empty;
        public BarcodeType BarcodeType { get; set; }
        public BarcodeSize Size { get; set; } = BarcodeSize.Small;
        public BarcodeForType BarcodeForType { get; set; }
        public string Content { get; set; } = string.Empty;
        public Guid? UserId { get; set; }
    }

    public record BarcodeResponseDto : BaseDto
    {
        public string Uid { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Data { get; set; } = string.Empty;
        public string Metadata { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public BarcodeType BarcodeType { get; set; }
    }
}