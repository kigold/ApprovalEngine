export interface Approval {
  requestId: number;
  approvalType: string;
  entityId: string;
  stage: string;
  stageOrder: string;
  status: string;
  version: number;
}
