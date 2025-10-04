using Core.Enums;

namespace Application.Dtos
{
    public record ProductCategoryRequestDto : BaseDto
    {
        public string? Uid { get; init; } = string.Empty;
        public string Name { get; init; } = string.Empty;
        public ProductCategoryType Type { get; init; }
        public bool Status { get; init; }
    }

    public record ProductCategoryResponseDto : BaseDto
    {
        public string? Uid { get; init; } = string.Empty;
        public string Name { get; init; } = string.Empty;
        public ProductCategoryType Type { get; init; }
        public string TypeTitle => Type.GetTitle();
        public bool Status { get; init; }
    }

    public record ProductCategoryStatusUpdateDto
    {
        public bool Status { get; init; }
        public Guid UpdatedBy { get; init; }
        public string? Reason { get; init; }
    }
}