using ApprovalEngine;
using ApprovalEngine.Enums;
using ApprovalEngine.Models;
using SampleApp.Core.Data.Entities;
using SampleApp.Core.Data.Repositories;
using SampleApp.Core.Models;

namespace SampleApp.Core.Services
{
    public class StudentService : IStudentService
    {
        private readonly IRepository<Student> _studentRepo;
        private readonly IApprovalService _approvalService;
        public StudentService(IRepository<Student> studentRepo, IApprovalService approvalService)
        {
            _studentRepo =  studentRepo;
            _approvalService = approvalService;
            _approvalService.OnApproval += OnApproval;
            _approvalService.OnDecline += OnDecline;
            _approvalService.OnReject += OnReject;
            _approvalService.OnReturn += OnReturn;
        }

        public async Task<ResultModel<bool>> CreateStudent(CreateStudentRequest model)
        {
            var student = new Student
            {
                FirstName = model.firstName,
                LastName = model.lastName,
                Email = model.email,
                Created = DateTime.Now,
                Status = Enums.StudentStatus.PendingApproval
            };
            _studentRepo.Insert(student);
            await _studentRepo.SaveChangesAsync();

            var approvalRequestResponse = await _approvalService.CreateApprovalRequest(new CreateApprovalRequest(ApprovalEngine.Enums.ApprovalType.StudentUser, student.Id.ToString()));
            if (approvalRequestResponse.HasError)
                return new ResultModel<bool>(approvalRequestResponse.ErrorMessages);

            return new ResultModel<bool>(true);
        }

        public async Task<ResultModel<bool>> DeleteStudent(long id)
        {
            _studentRepo.Delete(id);

            await _studentRepo.SaveChangesAsync();

            return new ResultModel<bool>(true);
        }

        public async Task<ResultModel<PagedList<StudentResponse>>> GetAllStudents(PagedRequestModel request)
        {
            var students = _studentRepo.Get().Select(x => (StudentResponse)x);

            return new ResultModel<PagedList<StudentResponse>>(new PagedList<StudentResponse>(students.Select(x => (StudentResponse)x), request.PageIndex, request.PageSize));
        }

        public async Task<ResultModel<StudentResponse>> GetStudent(long id)
        {
            var student = _studentRepo.GetByID(id);
            return new ResultModel<StudentResponse>(student);
        }

        public async Task<ResultModel<bool>> UpdateStudent(UpdateStudentRequest model)
        {
            var student = _studentRepo.GetByID(model.studentId);
            student.FirstName = model.firstName;
            student.LastName = model.lastName;
            student.Modified = DateTime.Now;

            _studentRepo.Update(student);
            return new ResultModel<bool>(true);
        }

        public Task<ResultModel<bool>> ApproveRequest(ApprovalModel model)
        {
            return _approvalService.ApproveRequest(model);
        }

        public Task<ResultModel<bool>> DeclineRequest(ApprovalModel model)
        {
            return _approvalService.DeclineRequest(model);
        }

        public Task<ResultModel<bool>> RejectRequest(ApprovalModel model)
        {
            return _approvalService.RejectRequest(model);
        }

        public Task<ResultModel<bool>> ReturnRequest(ApprovalModel model)
        {
            return _approvalService.ReturnRequest(model);
        }

        public Task<ResultModel<bool>> UpdateRequestToPending(long approvalRequestId)
        {
            return _approvalService.UpdateRequestToPending(approvalRequestId);
        }

        public Task<ResultModel<ApprovalRequestResponse>> GetRequest(long approvalRequestId)
        {
            return _approvalService.GetRequest(approvalRequestId);
        }

        public Task<ResultModel<PagedList<ApprovalRequestResponse>>> GetRequests(GetApprovalsRequest request)
        {
            return _approvalService.GetRequests(request);
        }

        public Task<ResultModel<PagedList<ApprovalRequestResponse>>> GetRequestsByPermission(GetApprovalsRequestByPermission request)
        {
            return _approvalService.GetRequestsByPermission(request);
        }

        public Task<ResultModel<List<ApprovalStageResponse>>> GetApprovalStages(GetApprovalStageRequest model)
        {
            return _approvalService.GetApprovalStages(model);
        }

        public Task<ResultModel<List<ApprovalStageByVersion>>> GetAllApprovalStages(ApprovalType approvalRequestType)
        {
            return _approvalService.GetAllApprovalStages(approvalRequestType);
        }

        public Task<ResultModel<List<ApprovalHistoryResponse>>> GetRequestsHistory(long approvalRequestId)
        {
            return _approvalService.GetRequestsHistory(approvalRequestId);
        }

        public Task<ResultModel<bool>> CreateRequestStages(CreateApprovalStage model)
        {
            return _approvalService.CreateRequestStages(model);
        }

        public Task<ResultModel<bool>> DeleteRequestStages(DeleteApprovalStages model)
        {
            return _approvalService.DeleteRequestStages(model);
        }

        //Event Callback
        private async Task OnApproval(bool isCompletelyApproved, string studentId)
        {
            //Logic to ba called after request is APPROVED goes here
            if (!isCompletelyApproved)
                return;

            var request = _studentRepo.GetByID(long.Parse(studentId));
            if (request == null)
                return;

            request.Status = Enums.StudentStatus.Active;
            _studentRepo.Update(request);
        }

        private async Task OnDecline(bool isCompletelyDeclined, string studentId)
        {
            //Logic to ba called after request is DECLINED goes here
            if (!isCompletelyDeclined)
                return;

            var request = _studentRepo.GetByID(long.Parse(studentId));
            if (request == null)
                return;

            request.Status = Enums.StudentStatus.Disabled;
            _studentRepo.Update(request);
        }

        private async Task OnReject(string studentId)
        {
            //Logic to ba called after request is REJECTED goes here
            var request = _studentRepo.GetByID(long.Parse(studentId));
            if (request == null)
                return;

            request.Status = Enums.StudentStatus.Disabled;
            _studentRepo.Update(request);
        }

        private async Task OnReturn(string studentId)
        {
            //Logic to ba called after request is RETURNED goes here
            var request = _studentRepo.GetByID(long.Parse(studentId));
            if (request == null)
                return;

            request.Status = Enums.StudentStatus.Created;
            _studentRepo.Update(request);
        }
    }
}
