﻿using SampleApp.Core.Data.Entities.ApprovalEngine;

namespace ApprovalEngine.Models
{
    public class ApprovalRequestResponse
    {
        public long RequestId { get; set; }
        public string ApprovalType { get; set; }
        public string EntityId { get; set; }
        public string Description { get; set; }
        public string Stage { get; set; }
        public int StatgeOrder { get; set; }
        public string Status { get; set; }
        public int Version { get; set; }
        public DateTime Created { get; set; }
        public string Creator { get; set; }

        public static implicit operator ApprovalRequestResponse(ApprovalRequest model)
        {
            return model == null ? null : new ApprovalRequestResponse
            {
                EntityId = model.EntityId,
                Description = model.Description,
                RequestId = model.Id,
                ApprovalType = model.ApprovalType.ToString(),
                Stage = model.Stage,
                StatgeOrder = model.StageOrder,
                Status = model.Status.ToString(),
                Version = model.Version,
                Created = model.Created,
                Creator = model.Creator.FullName
            };
        }
    }
}
