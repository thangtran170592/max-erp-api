using System.ComponentModel.DataAnnotations;
using Core.Enums;

namespace Application.Dtos
{
    public record ApprovalFeatureRequestDto : BaseDto
    {
        public string? Uid { get; set; }
        [Required]
        public string Name { get; set; } = string.Empty;
        [Required]
        public Guid ApprovalConfigId { get; set; }
        public List<ApprovalStepRequestDto> ApprovalSteps { get; set; } = [];
        public bool Status { get; set; } = true;
    }

    public record ApprovalFeatureResponseDto : BaseDto
    {
        public string Uid { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public Guid ApprovalConfigId { get; set; }
        public string ApprovalConfigName { get; set; } = string.Empty;
        public bool Status { get; set; }
        public List<ApprovalStepResponseDto> ApprovalSteps { get; set; } = [];
    }

    public record UpdateApprovalFeatureStatusRequestDto
    {
        public bool Status { get; set; }
        public Guid? UpdatedBy { get; set; }
    }
}
