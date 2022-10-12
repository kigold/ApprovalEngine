namespace ApprovalEngine.Models
{
    public class PagedList <T>
    {
        public long TotalCount { get; set; }
        public List<T> Items { get; set; }
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
    }
}
