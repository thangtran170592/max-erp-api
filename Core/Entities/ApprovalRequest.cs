using Core.Enums;

namespace Core.Entities
{
    public class ApprovalRequest : BaseEntity
    {
        public Guid ApprovalFeatureId { get; set; }
        public ApprovalFeature ApprovalFeature { get; set; } = null!;
        public Guid DataId { get; set; }
        public int CurrentStepOrder { get; set; }
        public ApprovalStatus Status { get; set; }
        public string? ReasonRejection { get; set; }
        public bool Editable { get; set; } = true;
        public Guid SubmitterId { get; set; }
        public virtual ApplicationUser Submitter { get; set; } = null!;
        public ICollection<ApprovalHistory> ApprovalHistories { get; set; } = [];
    }
}