using Core.Enums;

namespace Core.Entities
{
    public class ApprovalStep : BaseEntity
    {
        public Guid ApprovalFeatureId { get; set; }
        public ApprovalFeature ApprovalFeature { get; set; } = null!;
        public int StepOrder { get; set; }
        public ApprovalTargetType TargetType { get; set; }
        public Guid TargetValue { get; set; }
        public bool IsFinalStep { get; set; } = false;
    }
}