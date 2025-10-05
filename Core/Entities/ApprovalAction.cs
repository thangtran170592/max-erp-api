using Core.Enums;

namespace Core.Entities
{
    public class ApprovalAction : BaseEntity
    {
        public Guid ApprovalInstanceId { get; set; }
        public ApprovalInstance ApprovalInstance { get; set; } = null!;
        public int StepOrder { get; set; }
        public Guid? ApproverId { get; set; }
        public DateTime? ApprovedAt { get; set; }
        public ApprovalStatus Status { get; set; }
        public string? Reason { get; set; }
    }
}