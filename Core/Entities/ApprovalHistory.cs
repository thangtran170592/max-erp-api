using Core.Enums;

namespace Core.Entities
{
    public class ApprovalHistory : BaseEntity
    {
        public Guid ApprovalRequestId { get; set; }
        public ApprovalRequest ApprovalRequest { get; set; } = null!;
        public int StepOrder { get; set; }
        public Guid? ApproverId { get; set; }
        public DateTime? ApprovedAt { get; set; }
        public ApprovalStatus Status { get; set; }
        public string? Reason { get; set; }
    }
}