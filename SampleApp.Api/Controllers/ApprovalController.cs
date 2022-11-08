using ApprovalEngine.Enums;
using ApprovalEngine.Models;
using Microsoft.AspNetCore.Mvc;
using SampleApp.Core.Models;
using SampleApp.Core.Services;

namespace SampleApp.Api.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class ApprovalController : BaseController
    {
        private readonly IStudentService _studentService;
        public ApprovalController(IStudentService studentService)
        {
            _studentService = studentService;
        }

        [HttpPost]
        [ProducesResponseType(typeof(ApiResponse<bool>), 200)]
        public async Task<IActionResult> ApproveRequest([FromBody] ApprovalModel model)
        {
            return await Process(() => _studentService.ApproveRequest(model));
        }

        [HttpPost]
        [ProducesResponseType(typeof(ApiResponse<bool>), 200)]
        public async Task<IActionResult> DeclineRequest([FromBody] ApprovalModel model)
        {
            return await Process(() => _studentService.DeclineRequest(model));
        }

        [HttpPost]
        [ProducesResponseType(typeof(ApiResponse<bool>), 200)]
        public async Task<IActionResult> RejectRequest([FromBody] ApprovalModel model)
        {
            return await Process(() => _studentService.RejectRequest(model));
        }

        [HttpPost]
        [ProducesResponseType(typeof(ApiResponse<bool>), 200)]
        public async Task<IActionResult> ReturnRequest([FromBody] ApprovalModel model)
        {
            return await Process(() => _studentService.ReturnRequest(model));
        }

        [HttpPost("{requestId}")]
        [ProducesResponseType(typeof(ApiResponse<bool>), 200)]
        public async Task<IActionResult> UpdateToPending(long requestId)
        {
            return await Process(() => _studentService.UpdateRequestToPending(requestId));
        }

        [HttpGet("{requestId}")]
        [ProducesResponseType(typeof(ApiResponse<ApprovalRequestResponse>), 200)]
        public async Task<IActionResult> GetApprovalRequest(long requestId)
        {
            return await Process(() => _studentService.GetRequest(requestId));
        }

        [HttpGet]
        [ProducesResponseType(typeof(ApiResponse<List<ApprovalRequestResponse>>), 200)]
        public async Task<IActionResult> GetApprovalRequest([FromQuery] GetApprovalsRequest model)
        {
            return await Process(() => _studentService.GetRequests(model));
        }

        [HttpGet("Permission")]
        [ProducesResponseType(typeof(ApiResponse<List<ApprovalRequestResponse>>), 200)]
        public async Task<IActionResult> GetApprovalRequest([FromQuery] GetApprovalsRequestByPermission model)
        {
            return await Process(() => _studentService.GetRequestsByPermission(model));
        }

        [HttpGet("{requestId}")]
        [ProducesResponseType(typeof(ApiResponse<List<ApprovalHistoryResponse>>), 200)]
        public async Task<IActionResult> GetRequestHistory(long requestId)
        {
            return await Process(() => _studentService.GetRequestsHistory(requestId));

        }

        [HttpGet]
        [ProducesResponseType(typeof(ApiResponse<List<ApprovalStageResponse>>), 200)]
        public async Task<IActionResult> GetApprovalStages([FromQuery] GetApprovalStageRequest model)
        {
            return await Process(() => _studentService.GetApprovalStages(model));
        }

        [HttpGet]
        [ProducesResponseType(typeof(ApiResponse<List<ApprovalStageResponse>>), 200)]
        public async Task<IActionResult> GetAllApprovalStages([FromQuery] ApprovalType approvalRequestType)
        {
            return await Process(() => _studentService.GetAllApprovalStages(approvalRequestType));
        }

        [HttpPost]
        [ProducesResponseType(typeof(ApiResponse<bool>), 200)]
        public async Task<IActionResult> CreateRequestStages([FromBody] CreateApprovalStage model)
        {
            return await Process(() => _studentService.CreateRequestStages(model));
        }

        [HttpPost]
        [ProducesResponseType(typeof(ApiResponse<bool>), 200)]
        public async Task<IActionResult> DeleteRequestStages([FromBody] DeleteApprovalStages model)
        {
            return await Process(() => _studentService.DeleteRequestStages(model));
        }
    }
}
