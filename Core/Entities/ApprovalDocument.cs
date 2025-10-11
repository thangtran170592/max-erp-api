using Core.Enums;

namespace Core.Entities
{
    public class ApprovalDocument : BaseEntity
    {
        public Guid ApprovalFeatureId { get; set; }
        public virtual ApprovalFeature ApprovalFeature { get; set; } = null!;
        public Guid DocumentId { get; set; }
        public int CurrentStepOrder { get; set; }
        public ApprovalStatus ApprovalStatus { get; set; }
        public bool Editable { get; set; } = true;
        public Guid? SubmitterId { get; set; }
        public virtual ApplicationUser? Submitter { get; set; }
        public Guid LatestApprovalHistoryId { get; set; }
        public ICollection<ApprovalHistory> ApprovalHistories { get; set; } = [];
    }
}