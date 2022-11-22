using ApprovalEngine.Enums;
using ApprovalEngine.Models;
using SampleApp.Core.Models;

namespace SampleApp.Core.Services
{
    public interface IStudentService
    {
        Task<ResultModel<StudentResponse>> GetStudent(long id);
        Task<ResultModel<PagedList<StudentResponse>>> GetAllStudents(PagedRequestModel request);
        Task<ResultModel<bool>> CreateStudent(CreateStudentRequest model);
        Task<ResultModel<bool>> UpdateStudent(UpdateStudentRequest model);
        Task<ResultModel<bool>> DeleteStudent(long id);

        Task<ResultModel<bool>> ApproveRequest(ApprovalModel model);
        Task<ResultModel<bool>> DeclineRequest(ApprovalModel model);
        Task<ResultModel<bool>> RejectRequest(ApprovalModel model);
        Task<ResultModel<bool>> ReturnRequest(ApprovalModel model); //Return back to the user for input
        Task<ResultModel<bool>> UpdateRequestToPending(long approvalRequestId); //Update request back to pending after user input

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
