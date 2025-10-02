namespace Application.Dtos
{
    public record PackageRequestDto : BaseDto
    {
        public string? Uid { get; init; } = string.Empty;
        public string Name { get; init; } = string.Empty;
        public bool Status { get; init; } = true;
    }

    public record PackageResponseDto : BaseDto
    {
        public string? Uid { get; init; } = string.Empty;
        public string Name { get; init; } = string.Empty;
        public bool Status { get; init; }
    }

    public record PackageStatusUpdateDto : BaseDto
    {
        public bool Status { get; init; }
        public string? Reason { get; init; }
    }
}