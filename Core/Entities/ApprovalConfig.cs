using Core.Enums;

namespace Core.Entities
{
    public class ApprovalConfig : BaseEntity
    {
        public string Uid { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public ApprovalGroup ApprovalGroup { get; set; }
        public Guid LatestApprovalFeatureId { get; set; }
        public bool Status { get; set; } = true;
        public virtual ICollection<ApprovalFeature> ApprovalFeatures { get; set; } = [];
    }
}