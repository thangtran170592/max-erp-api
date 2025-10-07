using Core.Enums;

namespace Application.Dtos
{
    public record ApprovalConfigDto : BaseDto
    {
        public string Uid { get; init; } = string.Empty;
        public string Name { get; init; } = string.Empty;
        public string Description { get; init; } = string.Empty;
        public bool Status { get; init; }
        public List<ApprovalFeatureDto> Features { get; init; } = new();
    }

    public record ApprovalFeatureDto : BaseDto
    {
        public Guid ApprovalConfigId { get; set; }
        public ApprovalConfigDto ApprovalConfig { get; set; } = null!;
        public bool Status { get; init; }
        public string Uid { get; init; } = string.Empty;
        public ApprovalTargetType TargetType { get; init; }
        public Guid TargetValue { get; init; }
        public List<ApprovalStepDto> Steps { get; init; } = new();
    }

    public record ApprovalStepDto : BaseDto
    {
        public Guid ApprovalFeatureId { get; set; }
        public ApprovalFeatureDto ApprovalFeature { get; set; } = null!;
        public int StepOrder { get; init; }
        public ApprovalTargetType TargetType { get; init; }
        public Guid TargetValue { get; init; }
    }

    // Renamed to avoid conflict with detailed ApprovalInstanceRequestDto used for instance management
    public record ApproveInstanceRequestDto : BaseDto
    {
        public Guid UserId { get; init; }
        public ApprovalStatus Status { get; init; }
        public string? Reason { get; init; }
    }
}