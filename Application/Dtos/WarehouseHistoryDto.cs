using Core.Enums;

namespace Application.Dtos
{
    public record WarehouseHistoryDto : BaseDto
    {
        public Guid WarehouseId { get; init; }
        public string Uid { get; init; } = string.Empty;
        public string Name { get; init; } = string.Empty;
        public bool Status { get; init; }
        public ApprovalStatus ApprovalStatus { get; init; }
        public string ApprovalStatusTitle => ApprovalStatus.GetTitle();
        public DateTime ChangedAt { get; init; }
        public string? ChangedBy { get; init; }
    }
}