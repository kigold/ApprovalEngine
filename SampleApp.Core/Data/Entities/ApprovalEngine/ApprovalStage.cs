using ApprovalEngine.Enums;

namespace SampleApp.Core.Data.Entities.ApprovalEngine
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
