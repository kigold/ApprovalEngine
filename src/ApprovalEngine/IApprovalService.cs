using ApprovalEngine.Enums;
using ApprovalEngine.Models;

namespace ApprovalEngine
{
    public interface IApprovalService
    {
        //Delegates
        event Func<bool, string, Task> OnApproval;
        event Func<bool, string, Task> OnDecline;
        event Func<string, Task> OnReject;
        event Func<string, Task> OnReturn;

        //Action
        Task<ResultModel<bool>> CreateApprovalRequest(CreateApprovalRequest model);
        Task<ResultModel<bool>> ApproveRequest(ApprovalModel model);
        Task<ResultModel<bool>> DeclineRequest(ApprovalModel model);
        Task<ResultModel<bool>> RejectRequest(ApprovalModel model);
        /* Return back to the user for input */
        Task<ResultModel<bool>> ReturnRequest(ApprovalModel model);
        /* Update request back to pending after user input */
        Task<ResultModel<bool>> UpdateRequestToPending(long approvalRequestId);

        //Gets
        Task<ResultModel<ApprovalRequestResponse>> GetRequest(long approvalRequestId);
        Task<ResultModel<PagedList<ApprovalRequestResponse>>> GetRequests(GetApprovalsRequest request);
        Task<ResultModel<PagedList<ApprovalRequestResponse>>> GetRequestsByPermission(GetApprovalsRequestByPermission request);
        Task<ResultModel<List<ApprovalHistoryResponse>>> GetRequestsHistory(long approvalRequestId);

        //Admin
        Task<ResultModel<List<ApprovalStageResponse>>> GetApprovalStages(GetApprovalStageRequest model);
        Task<ResultModel<List<ApprovalStageByVersion>>> GetAllApprovalStages(ApprovalType approvalRequestType);
        Task<ResultModel<bool>> CreateRequestStages(CreateApprovalStage model);
        Task<ResultModel<bool>> DeleteRequestStages(DeleteApprovalStages model);
    }
}
