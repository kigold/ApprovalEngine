export interface Approval {
  requestId: number;
  approvalType: string;
  entityId: string;
  description: string;
  stage: string;
  stageOrder: string;
  status: string;
  version: number;
}

export interface ApprovalHistory {
  approvalId: number;
  action: string;
  creator: string;
  stage: string;
  stageOrder: number;
  dateTime: string;
  comment: string;
}

export interface GetApprovalsQuery {
  entityId?: string;
  ApprovalRequestType: ApprovalType;
  isPending?: boolean;
  isReturned?: boolean;
  pageIndex: number;
  pageSize: number;
}

export interface ApprovalModel {
  approvalRequestId: number;
  stage: string;
  comment: string;
}

export interface GetApprovalStageRequest {
  version: number;
  approvalRequestType: ApprovalType;
}

export interface ApprovalStageResponse {
  permission: string;
  stage: string;
  order: number;
  declineToOrder: number;
  approvalType: string;
  version: number;
}

export interface ApprovalStageByVersion {
  version: number;
  stages: ApprovalStageResponse[];
}

export interface CreateApprovalStage {
  approvalRequestType: ApprovalType;
  stages: ApprovalStageModel;
}

export interface DeleteApprovalStages {
  approvalRequestType: ApprovalType;
  version: number;
}

export interface ApprovalStageModel {
  permission: Permission;
  stage: string;
  order: number;
  declineToOrder: number;
}

export enum Permission {
  HOD,
  Approver,
  Admin,
  IT,
}

export enum ApprovalType {
  StudentUser,
  AdminUser,
  Teacher,
}
