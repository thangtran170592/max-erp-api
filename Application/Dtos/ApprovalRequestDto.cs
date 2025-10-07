using System.ComponentModel.DataAnnotations;
using Core.Enums;

namespace Application.Dtos;

public record class ApprovalRequestDto
{
    [Required]
    public Guid ApprovalFeatureId { get; set; }
    [Required]
    public Guid DataId { get; set; }
    public int CurrentStepOrder { get; set; } = 1; // start at 1 by default
    public ApprovalStatus Status { get; set; }
    [MaxLength(500)]
    public string? ReasonRejection { get; set; }
    public Guid? CreatedBy { get; set; }
    public Guid? UpdatedBy { get; set; }
}

public record class ApprovalResponseDto : BaseDto
{
    public Guid ApprovalFeatureId { get; set; }
    public Guid DataId { get; set; }
    public int CurrentStepOrder { get; set; }
    public ApprovalStatus Status { get; set; }
    public string? ReasonRejection { get; set; }
}

public record class UpdateApprovalInstanceStatusRequestDto
{
    [Required]
    public ApprovalStatus Status { get; set; }
    [MaxLength(500)]
    public string? ReasonRejection { get; set; }
    public Guid? UpdatedBy { get; set; }
}

public record class UpdateApprovalInstanceCurrentStepRequestDto
{
    [Required]
    public int CurrentStepOrder { get; set; }
    public Guid? UpdatedBy { get; set; }
}
