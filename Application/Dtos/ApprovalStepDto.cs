using System.ComponentModel.DataAnnotations;
using Application.Common.Models;
using Core.Enums;

namespace Application.Dtos
{
    public record ApprovalStepRequestDto
    {
        [Required]
        public Guid ApprovalFeatureId { get; set; }
        // Optional: if omitted (0 or null) we auto-calculate next order
        public int? StepOrder { get; set; }
        [Required]
        public ApprovalTargetType TargetType { get; set; }
        [Required]
        public Guid TargetValue { get; set; }
        public bool IsFinalStep { get; set; } = false;
        public Guid? CreatedBy { get; set; }
        public Guid? UpdatedBy { get; set; }
    }

    public record ApprovalStepResponseDto : BaseDto
    {
        public Guid ApprovalFeatureId { get; set; }
        public int StepOrder { get; set; }
        public ApprovalTargetType TargetType { get; set; }
        public Guid TargetValue { get; set; }
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
