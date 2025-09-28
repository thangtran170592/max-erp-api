using Core.Enums;

namespace Core.Entities
{
    public class WarehouseHistory : BaseEntity
    {
        public Guid WarehouseId { get; set; }
        public Guid Uid { get; set; }
        public string Name { get; set; } = string.Empty;
        public bool Status { get; set; }
        public ApprovalStatus ApprovalStatus { get; set; }
        public DateTime ChangedAt { get; set; }
        public string? ChangedBy { get; set; }

        public Warehouse? Warehouse { get; set; }
    }
}