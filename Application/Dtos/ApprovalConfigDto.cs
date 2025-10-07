using System.ComponentModel.DataAnnotations;
using Application.Common.Models;

namespace Application.Dtos
{
    public record ApprovalConfigRequestDto
    {
        [Required]
        [MaxLength(255)]
        public string Uid { get; set; } = string.Empty;
        [Required]
        [MaxLength(255)]
        public string Name { get; set; } = string.Empty;
        [MaxLength(1000)]
        public string? Description { get; set; }
        public bool Status { get; set; } = true;
        public Guid? CreatedBy { get; set; }
        public Guid? UpdatedBy { get; set; }
    }

    public record ApprovalConfigResponseDto : BaseDto
    {
        public string Uid { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public bool Status { get; set; }
    }

    public record UpdateApprovalConfigStatusRequestDto
    {
        public bool Status { get; set; }
        public Guid? UpdatedBy { get; set; }
    }
}
