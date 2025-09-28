namespace Application.Dtos
{
    public record class BaseDto
    {
        public Guid Id { get; init; }
        public DateTime CreatedAt { get; init; }
        public DateTime UpdatedAt { get; init; }
    }

    public record class ApiResponse<T>
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public T? Data { get; set; }
        public List<ApiError>? Errors { get; set; }
        public MetaData? Meta { get; set; }
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
        public string? TraceId { get; set; }
    }

    public record class ApiError
    {
        public string? Field { get; set; }
        public string? Message { get; set; }
    }

    public record class MetaData
    {
        public int? Page { get; set; }
        public int? PageSize { get; set; }
        public int? TotalCount { get; set; }
        public int? TotalPages { get; set; }
    }
}
