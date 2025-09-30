using Core.Enums;

namespace Core.Entities
{
    public class WarehouseHistory : BaseEntity
    {
        public Guid WarehouseId { get; set; }
        public required string Uid { get; set; }
        public string Name { get; set; } = string.Empty;
        public string ReasonRejection { get; set; } = string.Empty;
        public bool Status { get; set; }
        public ApprovalStatus ApprovalStatus { get; set; }
        public virtual Warehouse? Warehouse { get; set; }
    }
}
