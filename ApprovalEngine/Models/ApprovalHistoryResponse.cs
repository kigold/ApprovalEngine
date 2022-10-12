using ApprovalEngine.Entities;

namespace ApprovalEngine.Models
{
    public class ApprovalHistoryResponse
    {
        public long ApprovalId { get; set; }
        public string Action { get; set; }
        public string User { get; set; }
        public string Stage { get; set; }
        public int StageOrder { get; set; }
        public DateTime DateTime { get; set; }

        public static implicit operator ApprovalHistoryResponse(ApprovalHistory model)
        {
            return model == null ? null : new ApprovalHistoryResponse
            {
                ApprovalId = model.ApprovalRequestId,
                Action = model.Action.ToString(),
                DateTime = model.Created,
                Stage = model.Stage,
                StageOrder = model.StageOrder,
                User = model.Creator
            };
        }
    }
}
