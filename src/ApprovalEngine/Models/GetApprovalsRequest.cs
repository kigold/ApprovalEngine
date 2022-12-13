using ApprovalEngine.Enums;

namespace ApprovalEngine.Models
{
    public class GetApprovalsRequest : PagedRequestModel
    {
        public string? EntityId { get; set; }
        public ApprovalType? ApprovalRequestType { get; set; }
        public bool? IsPending { get; set; }
        public bool? IsReturned { get; set; }
        public string? Stage { get; set; }
    }
    public class GetApprovalsRequestByPermission : PagedRequestModel
    {
        public Permission Permission { get; set; }
    }
}
