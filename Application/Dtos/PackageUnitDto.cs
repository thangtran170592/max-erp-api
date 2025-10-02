namespace Application.Dtos
{
    public record PackageUnitRequestDto : BaseDto
    {
        public string Uid { get; init; } = string.Empty;
        public int Level { get; init; }
        public decimal Quantity { get; init; }
        public Guid UnitId { get; init; }
        public bool Status { get; init; }
    }

    public record PackageUnitResponseDto : BaseDto
    {
        public string Uid { get; init; } = string.Empty;
        public int Level { get; init; }
        public decimal Quantity { get; init; }
        public Guid UnitId { get; init; }
        public bool Status { get; init; }
    }

    public record PackageUnitStatusUpdateDto
    {
        public bool Status { get; init; }
        public Guid UpdatedBy { get; set; }
        public string? Reason { get; init; }
    }
}