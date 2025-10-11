using Core.Enums;

namespace Core.Entities
{
    public class ApprovalHistory : BaseEntity
    {
        public Guid ApprovalDocumentId { get; set; }
        public ApprovalDocument ApprovalDocument { get; set; } = null!;
        public int StepOrder { get; set; }
        public Guid? ApproverId { get; set; }
        public DateTime? ApprovedAt { get; set; }
        public ApprovalStatus ApprovalStatus { get; set; }
        public string? Comment { get; set; }
    }
}