using ApprovalEngine.Enums;

namespace ApprovalEngine.Entities
{
    public class ApprovalHistory : Entity
    {
        public long ApprovalRequestId { get; set; }
        public ActionType Action { get; set; }
        public string Stage { get; set; }
        public int StageOrder { get; set; }
        public ApprovalRequest Request { get; set; }
    }
}
