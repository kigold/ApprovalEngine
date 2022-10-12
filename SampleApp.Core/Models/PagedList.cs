namespace ApprovalEngine.Models
{
    public class PagedList<T>
    {
        public long TotalCount { get; set; }
        public List<T> Items { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }

        public PagedList(IQueryable<T> superset, int pageNumber, int pageSize)
        {
            TotalCount = superset == null ? 0 : superset.Count();
            PageSize = pageSize;
            PageNumber = pageNumber;
            PageSize = TotalCount > 0
                        ? (int) Math.Ceiling(TotalCount / (double) PageSize)
                        : 0;

            if (superset != null && TotalCount > 0)
                Items = superset.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();
            else
                Items = new List<T>();
        }
    }
}
