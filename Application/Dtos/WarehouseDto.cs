using Core.Enums;

namespace Application.Dtos
{
    public record WarehouseDto : BaseDto
    {
        public Guid Uid { get; init; }
        public string Name { get; init; } = string.Empty;
        public bool Status { get; init; }
        public string StatusTitle { get; init; } = string.Empty;
        public ApprovalStatus ApprovalStatus { get; init; }
        public string ApprovalStatusTitle => ApprovalStatus.GetTitle();
    }
}