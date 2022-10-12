using ApprovalEngine.Enums;

namespace ApprovalEngine.Models
{
    public record DeleteApprovalStages(
            ApprovalType ApprovalType,
            int Version
        );
}
