using Core.Enums;

namespace Application.Dtos;

public record InitializeApprovalRequestDto : BaseDto
{
    public ApprovalGroup ApprovalGroup { get; init; }
    public Guid ApprovalFeatureId { get; init; }
    public Guid DocumentId { get; init; }
}

public record ApprovalHandlerRequestDto : BaseDto
{
    public ApprovalStatus ApprovalStatus { get; init; }
    public Guid ApproverId { get; set; }
    public Guid DocumentId { get; set; }
    public string Comment { get; set; } = string.Empty;
}

public record UpdateApprovalRequestDto : BaseDto
{
    public ApprovalStatus ApprovalStatus { get; init; }
    public string Comment { get; init; } = string.Empty;
}