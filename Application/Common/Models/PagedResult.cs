namespace Application.Common.Models
{
    public class PagedResult<T>
    {
        public List<T> Data { get; set; } = [];
        public int TotalCount { get; set; }
        public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);
        public int Page { get; set; }
        public int PageSize { get; set; }
    }
}