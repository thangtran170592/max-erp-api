using System.ComponentModel.DataAnnotations;
using Application.Common.Models;
using Core.Enums;

namespace Application.Dtos
{
    public record ApprovalStepRequestDto : BaseDto
    {
        [Required]
        public int StepOrder { get; set; }
        public ApprovalTargetType TargetType { get; set; }
        public Guid TargetId { get; set; }
        public bool IsFinalStep { get; set; } = false;
    }

    public record ApprovalStepResponseDto : BaseDto
    {
        public Guid ApprovalFeatureId { get; set; }
        public int StepOrder { get; set; }
        public ApprovalTargetType TargetType { get; set; }
        public Guid TargetId { get; set; }
        public bool IsFinalStep { get; set; }
    }

    public record UpdateApprovalStepOrderRequestDto
    {
        public int StepOrder { get; set; }
        public Guid? UpdatedBy { get; set; }
    }

    public record UpdateApprovalStepStatusRequestDto // For toggling final step flag
    {
        public bool IsFinalStep { get; set; }
        public Guid? UpdatedBy { get; set; }
    }
}
