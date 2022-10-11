﻿using ApprovalEngine.Enums;

namespace ApprovalEngine.Models
{
    public class GetApprovalsRequest : PagedRequestModel
    {
        public long? EntityId { get; set; }
        public ApprovalType? ApprovalRequestType { get; set; }
        public bool? IsPending { get; set; }
        public bool? IsReturned { get; set; }
    }
    public class GetApprovalsRequestByPermission : PagedRequestModel
    {
        public Permission Permission { get; set; }
    }
}
