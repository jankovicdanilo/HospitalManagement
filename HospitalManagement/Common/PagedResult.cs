namespace HospitalManagement.Common
{
    public class PagedResult<T>
    {
        public List<T> Items { get; set; } = null;
        public int TotalCount { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);

        public static PagedResult<T> Create(List<T> items, int totalCount, int pageNumber, int pageSize)
        {
            return new PagedResult<T>
            {
                Items = items,
                TotalCount = totalCount,
                PageNumber = pageNumber,
                PageSize = pageSize
            };
        }
    }
}
