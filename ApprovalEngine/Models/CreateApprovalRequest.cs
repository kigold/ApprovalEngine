using ApprovalEngine.Enums;

namespace ApprovalEngine.Models
{
    public record CreateApprovalRequest(ApprovalType ApprovalType, string EntityId);
}
