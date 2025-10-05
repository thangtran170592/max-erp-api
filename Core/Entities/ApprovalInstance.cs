using Core.Enums;

namespace Core.Entities
{
    public class ApprovalInstance : BaseEntity
    {
        public Guid ApprovalFeatureId { get; set; }
        public ApprovalFeature ApprovalFeature { get; set; } = null!;
        public Guid DataId { get; set; }
        public int CurrentStepOrder { get; set; }
        public ApprovalStatus Status { get; set; }
        public string? ReasonRejection { get; set; }
        public ICollection<ApprovalAction> Actions { get; set; } = [];
    }
}