namespace Application.Dtos
{
    public record ProductResponseDto : BaseDto
    {
        public string Uid { get; init; } = string.Empty;
        public string Name { get; init; } = string.Empty;
        public decimal Price { get; init; }
        public bool Status { get; init; }
        public Guid CategoryId { get; init; }
        public Guid PackageId { get; init; }
        public Guid UnitOfMeasureId { get; init; }
    }

    public record ProductRequestDto : BaseDto
    {
        public string Uid { get; init; } = string.Empty;
        public string Name { get; init; } = string.Empty;
        public decimal Price { get; init; }
        public Guid CategoryId { get; init; }
        public Guid PackageId { get; init; }
        public Guid UnitOfMeasureId { get; init; }
        public bool Status { get; init; } = true;
    }

    public record ProductStatusUpdateDto
    {
        public bool Status { get; init; }
        public string? Reason { get; init; }
        public Guid UpdatedBy { get; init; }
    }
}
