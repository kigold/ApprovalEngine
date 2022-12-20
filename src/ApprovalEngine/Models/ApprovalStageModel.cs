using ApprovalEngine.Enums;
using SampleApp.Core.Data.Entities.ApprovalEngine;

namespace ApprovalEngine.Models
{
    public record ApprovalStageModel(
            Permission Permission,
            string Stage,
            int Order,
            /* Order of the stage that the flow should go to when declined */
            int DeclineToOrder
        );

    public record CreateApprovalStage(
            ApprovalType ApprovalRequestType,
            List<ApprovalStageModel> Stages
        );

    public record GetApprovalStageRequest(
            int Version,
            ApprovalType ApprovalRequestType
        );

    public class ApprovalStageResponse
    {
        public string Permission { get; set; }
        public string Stage { get; set; }
        public int Order { get; set; }
        public int DeclineToOrder { get; set; } // Order of the stage that the flow should go to when declined
        public string ApprovalType { get; set; }
        public int Version { get; set; }

        public static implicit operator ApprovalStageResponse(ApprovalStage model)
        {
            return model == null ? null : new ApprovalStageResponse
            {
                ApprovalType = model.ApprovalType.ToString(),
                Permission = model.Permission.ToString(),
                DeclineToOrder = model.DeclineToOrder,
                Order = model.StageOrder,
                Stage = model.Name,
                Version = model.Version
            };
        }
    }

    public record ApprovalStageByVersion(int Version, IEnumerable<ApprovalStageResponse> Stages);

    public record ApprovalTypeResponse(string Name, int VersionCount);
}
