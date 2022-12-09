using ApprovalEngine.Enums;

namespace ApprovalEngine.Models
{
    public record DeleteApprovalStages(
            ApprovalType ApprovalRequestType,
            int Version
        );
}
