using Core.Enums;

namespace Application.Dtos
{
    public record WarehouseHistoryDto : BaseDto
    {
        public Guid WarehouseId { get; init; }
        public Guid Uid { get; init; }
        public string Name { get; init; } = string.Empty;
        public bool Status { get; init; }
        public ApprovalStatus ApprovalStatus { get; init; }
        public string ApprovalStatusTitle => ApprovalStatus.GetTitle();
        public DateTime ChangedAt { get; init; }
        public string? ChangedBy { get; init; }
    }
}