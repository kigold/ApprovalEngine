namespace ApprovalEngine.Models
{
    public class PagedRequestModel
    {
        private int _pageIndex;
        private int _pageSize;

        public int PageIndex
        {
            get => _pageIndex == 0 ? 1 : _pageIndex; 
            set => _pageIndex = value;
            //set => _pageIndex = value < 1 ? 1 : value; 
        }
        public int PageSize
        {
            get => _pageSize == 0 ? 20 : _pageSize;
            set => _pageSize = value;
            //set => _pageSize = value > 20 ? 20 : value;
        }
    }

    public class QueryModel
    {
        public string Query { get; set; }
    }
}
