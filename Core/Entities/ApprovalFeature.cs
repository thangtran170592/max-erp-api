using Core.Enums;

namespace Core.Entities
{
    public class ApprovalFeature : BaseEntity
    {
        public string Uid { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public Guid ApprovalConfigId { get; set; }
        public ApprovalConfig ApprovalConfig { get; set; } = null!;
        public bool Status { get; set; } = true;
        public ApprovalTargetType TargetType { get; set; }
        public Guid TargetId { get; set; }
        public ICollection<ApprovalStep> ApprovalSteps { get; set; } = [];

    }
}