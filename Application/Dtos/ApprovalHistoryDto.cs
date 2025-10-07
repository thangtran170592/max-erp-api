using System.ComponentModel.DataAnnotations;
using Core.Enums;

namespace Application.Dtos;

public record class ApprovalHistoryRequestDto
{
    [Required]
    public Guid ApprovalInstanceId { get; set; }
    public int? StepOrder { get; set; }
    public Guid? ApproverId { get; set; }
    public ApprovalStatus Status { get; set; }
    [MaxLength(500)]
    public string? Reason { get; set; }
    public Guid? CreatedBy { get; set; }
    public Guid? UpdatedBy { get; set; }
}

public record class ApprovalHistoryResponseDto : BaseDto
{
    public Guid ApprovalInstanceId { get; set; }
    public int StepOrder { get; set; }
    public Guid? ApproverId { get; set; }
    public DateTime? ApprovedAt { get; set; }
    public ApprovalStatus Status { get; set; }
    public string? Reason { get; set; }
}

public record class UpdateApprovalActionStatusRequestDto
{
    [Required]
    public ApprovalStatus Status { get; set; }
    [MaxLength(500)]
    public string? Reason { get; set; }
    public Guid? UpdatedBy { get; set; }
}

public record class UpdateApprovalActionOrderRequestDto
{
    [Required]
    public int StepOrder { get; set; }
    public Guid? UpdatedBy { get; set; }
}
