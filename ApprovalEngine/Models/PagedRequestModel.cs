namespace ApprovalEngine.Models
{
    public class PagedRequestModel
    {
        public int PageIndex { get; set; } = 1;
        int PageSize { get; set; } = 10;
    }

    public class QueryModel
    {
        public string Query { get; set; }
    }
}
