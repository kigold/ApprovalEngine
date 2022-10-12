using ApprovalEngine.Enums;

namespace SampleApp.Core.Data.Entities.ApprovalEngine
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
