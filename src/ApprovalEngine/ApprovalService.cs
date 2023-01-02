using ApprovalEngine.Enums;
using ApprovalEngine.Models;
using SampleApp.Core;
using SampleApp.Core.Data.Entities.ApprovalEngine;
using SampleApp.Core.Data.Repositories;

namespace ApprovalEngine
{
    public class ApprovalService : IApprovalService
    {
        private readonly IHttpUserService _httpUserService;
        private readonly IRepository<ApprovalRequest> _approvalRepository;
        private readonly IRepository<ApprovalStage> _approvalStageRepository;
        private readonly IRepository<ApprovalHistory> _approvalHistoryRepository;
        public event Func<bool, string, Task> OnApproval;
        public event Func<bool, string, Task> OnDecline;
        public event Func<string, Task> OnReject;
        public event Func<string, Task> OnReturn;

        public ApprovalService(
            IRepository<ApprovalRequest> approvalRepository,
            IRepository<ApprovalStage> approvalStageRepository,
            IRepository<ApprovalHistory> approvalHistoryRepository,
            IHttpUserService httpUserService)
        {
            _approvalHistoryRepository = approvalHistoryRepository;
            _approvalRepository = approvalRepository;
            _approvalStageRepository = approvalStageRepository;
            _httpUserService = httpUserService;
        }

        #region Action

        public async Task<ResultModel<bool>> CreateApprovalRequest(CreateApprovalRequest model)
        {
            //Get First Stage
            var getAllStagesForApprovalTypeQuery = _approvalStageRepository.Get().Where(x => x.ApprovalType == model.ApprovalType);
            var stagesQuery = getAllStagesForApprovalTypeQuery.Where(x => x.Version == getAllStagesForApprovalTypeQuery.Max(s => (int?)s.Version))//Get latest stages
                                                     .ToList();

            var firstStage = stagesQuery?.OrderBy(x => x.StageOrder)?.FirstOrDefault();

            if (firstStage == null)
                return new ResultModel<bool>("Approval Stages are not configured.");

            var request = new ApprovalRequest
            {
                EntityId = model.EntityId,
                Description = model.Description,
                ApprovalType = model.ApprovalType,
                Stage = firstStage.Name,
                StageOrder = firstStage.StageOrder,
                Status = ApprovalStatus.Created,
                Version = firstStage.Version,
                Created= DateTime.UtcNow,
                CreatedBy = _httpUserService.GetCurrentUser().UserId
            };

            _approvalRepository.Insert(request);
            await _approvalRepository.SaveChangesAsync();

            _approvalHistoryRepository.Insert(new ApprovalHistory
            {
                Action = ActionType.Create,
                ApprovalRequestId = request.Id,
                Stage = "Created",
                Created = DateTime.Now,
                CreatedBy = _httpUserService.GetCurrentUser().UserId
            });

            await _approvalRepository.SaveChangesAsync();

            return new ResultModel<bool>(true, "Success");
        }

        public async Task<ResultModel<bool>> ApproveRequest(ApprovalModel model)
        {
            var request = _approvalRepository.GetByID(model.ApprovalRequestId);

            if (request == null)
                return new ResultModel<bool>($"Approval Request with Id {model.ApprovalRequestId} not found.");

            if (!IsPendingApprovalStatus(request.Status))
                return new ResultModel<bool>($"Request is not pending approval");

            if (!request.Stage.Equals(model.Stage, StringComparison.OrdinalIgnoreCase))
                return new ResultModel<bool>($"Request stage does not match.");

            _approvalHistoryRepository.Insert(new ApprovalHistory
            {
                Action = ActionType.Approve,
                ApprovalRequestId = request.Id,
                Stage = request.Stage,
                StageOrder = request.StageOrder,
                Comment = model.Comment,
                Created = DateTime.Now,
                CreatedBy = _httpUserService.GetCurrentUser().UserId
            });

            //Get Stages
            var nextStage = _approvalStageRepository.Get(x => x.ApprovalType == request.ApprovalType && x.Version == request.Version,
                                                        o => o.OrderBy(x => x.StageOrder))
                                                    .FirstOrDefault(x => x.StageOrder > request.StageOrder);

            var isLastStage = nextStage == null;
            if (isLastStage) //No next stage hence this is final stage
            {
                request.Status = ApprovalStatus.Approved;
            }
            else
            {
                request.Stage = nextStage.Name;
                request.StageOrder = nextStage.StageOrder;
                request.Status = ApprovalStatus.Pending;
            }

            await _approvalRepository.SaveChangesAsync();

            OnApproval?.Invoke(isLastStage, request.EntityId);

            return new ResultModel<bool>(true, "Success");
        }

        public async Task<ResultModel<bool>> DeclineRequest(ApprovalModel model)
        {
            var request = _approvalRepository.GetByID(model.ApprovalRequestId);

            if (request == null)
                return new ResultModel<bool>($"Approval Request with Id {model.ApprovalRequestId} not found");

            if (!IsPendingApprovalStatus(request.Status))
                return new ResultModel<bool>($"Request is not pending approval");

            if (!request.Stage.Equals(model.Stage, StringComparison.OrdinalIgnoreCase))
                return new ResultModel<bool>($"Request stage does not match.");

            _approvalHistoryRepository.Insert(new ApprovalHistory
            {
                Action = ActionType.Decline,
                ApprovalRequestId = request.Id,
                Stage = request.Stage,
                StageOrder = request.StageOrder,
                Comment = model.Comment,
                Created = DateTime.Now,
                CreatedBy = _httpUserService.GetCurrentUser().UserId
            });

            //Get Stages
            var stages = _approvalStageRepository.Get(x => x.ApprovalType == request.ApprovalType && x.Version == request.Version);
            var currentStage = stages.FirstOrDefault(x => x.StageOrder == request.StageOrder);

            if (currentStage == null)
                return new ResultModel<bool>($"Approval Stage with order {request.StageOrder} for request {model.ApprovalRequestId} is not found");

            var isFirstStage = currentStage.DeclineToOrder == request.StageOrder;
            if (isFirstStage) //this is the first stage
            {
                request.Status = ApprovalStatus.Rejected;
            }
            else
            {
                var newStage = stages.FirstOrDefault(x => x.StageOrder == currentStage.DeclineToOrder);
                request.Status = ApprovalStatus.Pending;
                request.Stage = newStage.Name;
                request.StageOrder = newStage.StageOrder;
            }

            await _approvalRepository.SaveChangesAsync();

            OnDecline?.Invoke(isFirstStage, request.EntityId);

            return new ResultModel<bool>(true, "Success");
        }

        public async Task<ResultModel<bool>> RejectRequest(ApprovalModel model)
        {
            var request = _approvalRepository.GetByID(model.ApprovalRequestId);

            if (request == null)
                return new ResultModel<bool>($"Approval Request with Id {model.ApprovalRequestId} not found");

            if (IsCompletedApprovalStatus(request.Status))
                return new ResultModel<bool>($"Request is aleady completed.");

            if (!request.Stage.Equals(model.Stage, StringComparison.OrdinalIgnoreCase))
                return new ResultModel<bool>($"Request stage does not match.");

            request.Status = ApprovalStatus.Rejected;

            _approvalHistoryRepository.Insert(new ApprovalHistory
            {
                Action = ActionType.Reject,
                ApprovalRequestId = request.Id,
                Stage = request.Stage,
                StageOrder = request.StageOrder,
                Comment = model.Comment,
                Created = DateTime.Now,
                CreatedBy = _httpUserService.GetCurrentUser().UserId
            });

            await _approvalRepository.SaveChangesAsync();

            OnReject?.Invoke(request.EntityId);

            return new ResultModel<bool>(true, "Success");
        }

        public async Task<ResultModel<bool>> ReturnRequest(ApprovalModel model)
        {
            var request = _approvalRepository.GetByID(model.ApprovalRequestId);

            if (request == null)
                return new ResultModel<bool>($"Approval Request with Id {model.ApprovalRequestId} not found");

            if (IsCompletedApprovalStatus(request.Status) || request.Status == ApprovalStatus.Returned)
                return new ResultModel<bool>($"Request is aleady completed.");

            if (!request.Stage.Equals(model.Stage, StringComparison.OrdinalIgnoreCase))
                return new ResultModel<bool>($"Request stage does not match.");

            request.Status = ApprovalStatus.Returned;

            _approvalHistoryRepository.Insert(new ApprovalHistory
            {
                Action = ActionType.Return,
                ApprovalRequestId = request.Id,
                Stage = request.Stage,
                StageOrder = request.StageOrder,
                Comment = model.Comment,
                Created = DateTime.Now,
                CreatedBy = _httpUserService.GetCurrentUser().UserId
            });

            await _approvalRepository.SaveChangesAsync();

            OnReturn?.Invoke(request.EntityId);

            return new ResultModel<bool>(true, "Success");
        }

        public async Task<ResultModel<bool>> UpdateRequestToPending(long approvalRequestId)
        {
            var request = _approvalRepository.GetByID(approvalRequestId);

            if (request == null)
                return new ResultModel<bool>($"Approval Request with Id {approvalRequestId} not found");

            if (request.Status != ApprovalStatus.Returned)
                return new ResultModel<bool>($"Request is currently not returned.");

            //Update Stage to first stage
            var stage = _approvalStageRepository.Get(x => x.ApprovalType == request.ApprovalType && x.Version == request.Version,
                                                        o => o.OrderBy(x => x.StageOrder))
                                                    .FirstOrDefault();
            request.Stage = stage.Name;
            request.StageOrder = stage.StageOrder;
            request.Status = ApprovalStatus.Pending;

            _approvalHistoryRepository.Insert(new ApprovalHistory
            {
                Action = ActionType.UpdateToPending,
                ApprovalRequestId = request.Id,
                Stage = request.Stage,
                StageOrder = request.StageOrder,
                Created = DateTime.Now,
                CreatedBy = _httpUserService.GetCurrentUser().UserId
            });

            await _approvalRepository.SaveChangesAsync();

            return new ResultModel<bool>(true, "Success");
        }

        #endregion

        #region Get

        public async Task<ResultModel<ApprovalRequestResponse>> GetRequest(long approvalRequestId)
        {
            //var request = _approvalRepository.GetByID(approvalRequestId);
            var request = _approvalRepository.Get(x => x.Id == approvalRequestId, includeProperties: "Creator").FirstOrDefault();

            if (request == null)
                return new ResultModel<ApprovalRequestResponse>("Request not found.");

            return new ResultModel<ApprovalRequestResponse>((ApprovalRequestResponse)request, "success");
        }

        public async Task<ResultModel<PagedList<ApprovalRequestResponse>>> GetRequests(GetApprovalsRequest request)
        {
            var queryable = _approvalRepository.Get(includeProperties: "Creator");

            if (request.EntityId != null)
                queryable = queryable.Where(x => x.EntityId == request.EntityId);
            if (request.ApprovalRequestType != null)
                queryable = queryable.Where(x => x.ApprovalType == request.ApprovalRequestType);
            if (request.IsPending != null)
            {
                queryable = queryable.Where(x => request.IsPending.Value ?
                    x.Status == ApprovalStatus.Created || x.Status == ApprovalStatus.Pending :
                    x.Status == ApprovalStatus.Approved || x.Status == ApprovalStatus.Rejected);
            }
            if (request.IsReturned != null && request.IsReturned.Value)
                queryable = queryable.Where(x => x.Status == ApprovalStatus.Returned);
            if ( !string.IsNullOrEmpty(request.Stage))
                queryable = queryable.Where(x => x.Stage.ToLower() == request.Stage.ToLower());

            return new ResultModel<PagedList<ApprovalRequestResponse>>(new PagedList<ApprovalRequestResponse>(queryable.Select(x => (ApprovalRequestResponse)x), request.PageIndex, request.PageSize));
        }

        public async Task<ResultModel<PagedList<ApprovalRequestResponse>>> GetRequestsByPermission(GetApprovalsRequestByPermission request)
        {
            //TODO convert to raw sql query to avoid loading all data into memory
            var query = (from approval in _approvalRepository.Get(includeProperties: "Creator")
                         join stage in _approvalStageRepository.Get().Where(x => x.Permission == request.Permission)
                         on approval.ApprovalType equals stage.ApprovalType
                         //on new { RequestType = approval.RequestType, StageOrder = approval.StageOrder }
                         //equals new
                         //{
                         //    RequsetType = stage.RequestType,
                         //    StageOrder = stage.StageOrder
                         //}
                         where approval.StageOrder == stage.StageOrder && approval.ApprovalType == stage.ApprovalType && approval.Version == stage.Version
                            && (approval.Status == ApprovalStatus.Created || approval.Status == ApprovalStatus.Pending) //Pending
                         select new
                         {
                             approval,
                             stage
                         }).AsQueryable();

            return new ResultModel<PagedList<ApprovalRequestResponse>>(new PagedList<ApprovalRequestResponse>(query.Select(x => (ApprovalRequestResponse)x.approval).AsQueryable(), request.PageIndex, request.PageSize));
        }

        public async Task<ResultModel<List<ApprovalHistoryResponse>>> GetRequestsHistory(long approvalRequestId)
        {
            var result = _approvalHistoryRepository.Get(x => x.ApprovalRequestId == approvalRequestId,
                                                        x => x.OrderByDescending(o => o.Created),
                                                        includeProperties: "Creator");

            return new ResultModel<List<ApprovalHistoryResponse>>(result.Select(x => (ApprovalHistoryResponse)x).ToList(), "success");
        }

        #endregion

        #region Admin

        public async Task<ResultModel<List<ApprovalTypeResponse>>> GetAllApprovalTypes()
        {
            var query = _approvalStageRepository.Get()
                .ToList()
                .GroupBy(x => x.ApprovalType);

            var result = new List<ApprovalTypeResponse>();
            foreach(var approvalType in query)
            {
                var group = approvalType.GroupBy(x => x.Version);
                result.Add(new ApprovalTypeResponse(approvalType.Key.ToString(), group.Count()));
            }
             
            return new ResultModel<List<ApprovalTypeResponse>>(result);
        }

        public async Task<ResultModel<List<ApprovalStageResponse>>> GetApprovalStages(GetApprovalStageRequest model)
        {
            var result = _approvalStageRepository.Get().Where(x => x.ApprovalType == model.ApprovalRequestType && x.Version == model.Version);

            return new ResultModel<List<ApprovalStageResponse>>(result.Select(x => (ApprovalStageResponse)x).ToList());
        }

        public async Task<ResultModel<List<ApprovalStageByVersion>>> GetAllApprovalStages(ApprovalType approvalRequestType)
        {
            var result = _approvalStageRepository.Get().Where(x => x.ApprovalType == approvalRequestType)
                .ToList()
                .GroupBy(x => x.Version);
                
            return new ResultModel<List<ApprovalStageByVersion>>(result.Select(x => new ApprovalStageByVersion(x.Key, x.Select(a => (ApprovalStageResponse)a))).ToList());
        }

        public async Task<ResultModel<bool>> CreateRequestStages(CreateApprovalStage model)
        {
            var validation = ValidateCreateApprovalStage(model);
            if (validation.HasError)
                return validation;

            var existingApprovalStage = _approvalStageRepository.Get(x => x.ApprovalType == model.ApprovalRequestType,
                                                   x => x.OrderByDescending(o => o.Version)).FirstOrDefault();

            var version = existingApprovalStage?.Version + 1 ?? 1;

            foreach (var stage in model.Stages)
            {
                _approvalStageRepository.Insert(new ApprovalStage
                {
                    ApprovalType = model.ApprovalRequestType.Value,
                    DeclineToOrder = stage.DeclineToOrder,
                    Name = stage.Stage,
                    Permission = stage.Permission,
                    StageOrder = stage.Order,
                    Version = version,
                    CreatedBy = _httpUserService.GetCurrentUser().UserId
                });
            }

            await _approvalStageRepository.SaveChangesAsync();

            return new ResultModel<bool>(true, "Success");
        }

        public async Task<ResultModel<bool>> DeleteRequestStages(DeleteApprovalStages model)
        {
            var approvalStages = _approvalStageRepository.Get().Where(x => x.ApprovalType == model.ApprovalRequestType && x.Version == model.Version).ToList();
            if (!approvalStages.Any())
                return new ResultModel<bool>("Approval Stages not found");

            //Check for pending Approval request dependent on the approvalsStages
            var hasPendingRequests = _approvalRepository.Get().Any(x => x.ApprovalType == model.ApprovalRequestType && x.Version == model.Version &&
                    (x.Status == ApprovalStatus.Created || x.Status == ApprovalStatus.Pending));//TODO Optimize query;
            if (hasPendingRequests)
                return new ResultModel<bool>("The selected Approval Stages have pending approval requests and cannot be deleted");

            approvalStages.ForEach(x => _approvalStageRepository.Delete(x));

            await _approvalStageRepository.SaveChangesAsync();

            return new ResultModel<bool>(true, "Success");
        }

        #endregion

        #region Private methods

        private ResultModel<bool> ValidateCreateApprovalStage(CreateApprovalStage model)
        {
            var resultModel = new ResultModel<bool>();
            if (model.Stages.Any(x => x.DeclineToOrder > x.Order))
                resultModel.AddError("Decline To Order cannot be less than Order");

            return resultModel;
        }

        private static bool FilterIsPendingRequest(ApprovalStatus status, bool isPending)
        {
            if (isPending)
            {
                return IsPendingApprovalStatus(status);
            }
            else
            {
                return IsCompletedApprovalStatus(status);
            }
        }

        private static bool IsPendingApprovalStatus(ApprovalStatus status)
        {
            if (status == ApprovalStatus.Created || status == ApprovalStatus.Pending)
                return true;
            return false;
        }

        private static bool IsCompletedApprovalStatus(ApprovalStatus status)
        {
            if (status == ApprovalStatus.Approved || status == ApprovalStatus.Rejected)
                return true;
            return false;

        }

        #endregion


















    }
}
