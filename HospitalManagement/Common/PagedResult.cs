namespace HospitalManagement.Common
{
    public class PagedResult<T>
    {
        public List<T> Items { get; set; } = null;
        public int TotalCount { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);
    }
}
