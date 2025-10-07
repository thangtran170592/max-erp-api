using System.ComponentModel.DataAnnotations;
using Core.Enums;

namespace Application.Dtos
{
    public record ApprovalFeatureRequestDto
    {
        [Required]
        [MaxLength(255)]
        public string Uid { get; set; } = string.Empty;
        [Required]
        public string Name { get; set; } = string.Empty;
        [Required]
        public Guid ApprovalConfigId { get; set; }
        [Required]
        public ApprovalTargetType TargetType { get; set; }
        [Required]
        public Guid TargetId { get; set; }
        public bool Status { get; set; } = true;
        public Guid? CreatedBy { get; set; }
        public Guid? UpdatedBy { get; set; }
    }

    public record ApprovalFeatureResponseDto : BaseDto
    {
        public string Uid { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public Guid ApprovalConfigId { get; set; }
        public string ApprovalConfigName { get; set; } = string.Empty;
        public ApprovalTargetType TargetType { get; set; }
        public string TargetTypeName => TargetType.GetTitle();
        public Guid TargetId { get; set; }
        public bool Status { get; set; }
    }

    public record UpdateApprovalFeatureStatusRequestDto
    {
        public bool Status { get; set; }
        public Guid? UpdatedBy { get; set; }
    }
}
