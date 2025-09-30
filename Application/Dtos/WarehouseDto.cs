using Application.Common.Models;
using Core.Enums;

namespace Application.Dtos
{
    public record WarehouseRequestDto : BaseDto
    {
        public string? Uid { get; init; } = string.Empty;
        public string Name { get; init; } = string.Empty;
        public bool Status { get; init; }
        public ApprovalStatus ApprovalStatus { get; init; }
    }

    public record WarehouseResponseDto : BaseDto
    {
        public string Uid { get; init; } = string.Empty;
        public string Name { get; init; } = string.Empty;
        public bool Status { get; init; }
        public string StatusTitle => Status ? "Active" : "Inactive";
        public ApprovalStatus ApprovalStatus { get; init; }
        public string ApprovalStatusTitle => ApprovalStatus.GetTitle();
    }

    public record WarehouseStatusUpdateDto : BaseDto
    {
        public ApprovalStatus ApprovalStatus { get; init; }
    }

    public record WarehouseFilterDto : BaseDto
    {
        public Dictionary<string, object>? Filters { get; init; }
        public PagedData? PagedData { get; init; }
    }
}