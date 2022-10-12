using ApprovalEngine.Enums;

namespace ApprovalEngine.Entities
{
    public class ApprovalStage : Entity
    {
        public string Name { get; set; }
        public ApprovalType ApprovalType { get; set; }
        public int StageOrder { get; set; }
        public int DeclineToOrder { get; set; } //On decline move to Approval Stage with this Order
        public Permission Permission { get; set; }
        public int Version { get; set; }
    }
}
