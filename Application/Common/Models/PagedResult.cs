namespace Application.Common.Models
{
    public class PagedData
    {
        public int TotalCount { get; set; }
        public int TotalPages => (int)Math.Ceiling((double)TotalCount / Take);
        public int Skip { get; set; }
        public int Take { get; set; }
    }
}