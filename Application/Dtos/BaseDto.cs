using Application.Common.Models;

namespace Application.Dtos
{
    public record class BaseDto
    {
        public Guid? Id { get; init; }
        public DateTime CreatedAt { get; set; }
        public Guid? CreatedBy { get; set; }
        public DateTime UpdatedAt { get; set; }
        public Guid? UpdatedBy { get; set; }
    }

    public record class ApiResponseDto<T>
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public T? Data { get; set; }
        public List<ApiErrorDto>? Errors { get; set; }
        public PagedData? PageData { get; set; }
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
        public string? TraceId { get; set; }
    }

    public record class ApiErrorDto
    {
        public string? Field { get; set; }
        public string? Message { get; set; }
    }

    public class FilterRequestDto
    {
        public Dictionary<string, object>? Filters { get; set; }
        public List<SortDto> Sorts { get; set; } = [new SortDto { Field = "CreatedAt", Dir = "desc" }];
        public string? SearchTerm { get; set; }
        public PageData PagedData { get; set; } = new PageData();
    }

    public class SortDto
    {
        public string Field { get; set; } = string.Empty;
        public string Dir { get; set; } = "asc"; // "asc" or "desc"
    }

    public class PageData
    {
        public int Skip { get; set; }
        public int Take { get; set; }
    }
}
