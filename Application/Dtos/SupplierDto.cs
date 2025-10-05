namespace Application.Dtos
{
    public record SupplierRequestDto : BaseDto
    {
        public string Uid { get; init; } = string.Empty;
        public string Name { get; init; } = string.Empty;
        public string Address { get; init; } = string.Empty;
        public string Tax { get; init; } = string.Empty;
        public string Phone { get; init; } = string.Empty;
        public string Email { get; init; } = string.Empty;
        public bool Status { get; init; }
    }

    public record SupplierResponseDto : BaseDto
    {
        public string Uid { get; init; } = string.Empty;
        public string Name { get; init; } = string.Empty;
        public string Address { get; init; } = string.Empty;
        public string Tax { get; init; } = string.Empty;
        public string Email { get; init; } = string.Empty;
        public string Phone { get; init; } = string.Empty;
        public bool Status { get; init; }
    }

    public record SupplierStatusUpdateDto
    {
        public string Uid { get; init; } = string.Empty;
        public bool Status { get; init; }
        public Guid UpdatedBy { get; set; }
        public string? Reason { get; init; }
    }
}