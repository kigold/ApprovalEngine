using ApprovalEngine.Enums;

namespace ApprovalEngine.Entities
{
    public class ApprovalRequest : Entity
    {
        public string EntityId { get; set; }
        public ApprovalType ApprovalType { get; set; }
        public int StageOrder { get; set; }
        public string Stage { get; set; }
        public ApprovalStatus Status { get; set; }
        public int Version { get; set; }
    }
}
