using Core.Enums;

namespace Core.Entities
{
    public class Warehouse : BaseEntity
    {
        public required string Uid { get; set; }
        public string Name { get; set; } = string.Empty;
        public bool Status { get; set; }
        public ApprovalStatus ApprovalStatus { get; set; }
    }
}