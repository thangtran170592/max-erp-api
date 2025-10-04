namespace Application.Dtos
{
    public record PackageUnitRequestDto : BaseDto
    {
        public Guid PackageId { get; init; }
        public int Level { get; init; }
        public decimal Quantity { get; init; }
        public Guid UnitId { get; init; }
    }

    public record PackageUnitResponseDto : BaseDto
    {
        public Guid PackageId { get; init; }
        public string PackageName { get; init; } = string.Empty;
        public int Level { get; init; }
        public decimal Quantity { get; init; }
        public Guid UnitId { get; init; }
        public string UnitName { get; init; } = string.Empty;
    }
}