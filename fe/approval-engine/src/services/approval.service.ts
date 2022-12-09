import { api } from 'boot/axios';
import {
  Approval,
  ApprovalHistory,
  ApprovalModel,
  ApprovalStageByVersion,
  ApprovalStageResponse,
  ApprovalType,
  CreateApprovalStage,
  DeleteApprovalStages,
  GetApprovalsQuery,
  GetApprovalStageRequest,
} from 'src/models/approval';
import { ResponseModel } from 'src/models/response.model';

const RequestOptions = () => {
  return {
    Accept: '*/*',
    //'Content-Type': 'application/json;charset=utf-8',
    Authorization: `Bearer ${localStorage.getItem('token')}`,
  };
};

export default class ApprovalService {
  static async GetApprovals(payload: GetApprovalsQuery) {
    return (
      await api.get<ResponseModel<Approval[]>>(
        '/api/Approval/GetApprovalRequest',
        {
          params: { ...payload },
          headers: RequestOptions(),
        }
      )
    ).data;
  }

  static async GetApproval(id: number) {
    return (
      await api.get<ResponseModel<Approval>>(
        `/api/Approval/GetApprovalRequest/${id}`,
        {
          headers: RequestOptions(),
        }
      )
    ).data;
  }

  static async GetRequestHistory(id: number) {
    return (
      await api.get<ResponseModel<ApprovalHistory[]>>(
        `/api/Approval/GetRequestHistory/${id}`,
        {
          headers: RequestOptions(),
        }
      )
    ).data;
  }

  static async ApproveRequest(payload: ApprovalModel) {
    return (
      await api.post<ResponseModel<boolean>>(
        '/api/Approval/ApproveRequest',
        payload,
        {
          headers: RequestOptions(),
        }
      )
    ).data;
  }

  static async DeclineRequest(payload: ApprovalModel) {
    return (
      await api.post<ResponseModel<boolean>>(
        '/api/Approval/DeclineRequest',
        payload,
        {
          headers: RequestOptions(),
        }
      )
    ).data;
  }

  static async RejectRequest(payload: ApprovalModel) {
    return (
      await api.post<ResponseModel<boolean>>(
        '/api/Approval/RejectRequest',
        payload,
        {
          headers: RequestOptions(),
        }
      )
    ).data;
  }

  static async ReturnRequest(payload: ApprovalModel) {
    return (
      await api.post<ResponseModel<boolean>>(
        '/api/Approval/ReturnRequest',
        payload,
        {
          headers: RequestOptions(),
        }
      )
    ).data;
  }

  static async UpdateToPending(id: number) {
    return (
      await api.post<ResponseModel<boolean>>(
        `/api/Approval/UpdateToPending/${id}`,
        {
          headers: RequestOptions(),
        }
      )
    ).data;
  }

  //Admin endpoints
  static async GetApprovalStages(payload: GetApprovalStageRequest) {
    return (
      await api.get<ResponseModel<ApprovalStageResponse[]>>(
        '/api/Approval/GetApprovalStages',
        {
          params: { payload },
          headers: RequestOptions(),
        }
      )
    ).data;
  }

  static async GetAllApprovalStages(payload: ApprovalType) {
    return (
      await api.get<ResponseModel<ApprovalStageByVersion[]>>(
        '/api/Approval/GetAllApprovalStages',
        {
          params: { payload },
          headers: RequestOptions(),
        }
      )
    ).data;
  }

  static async CreateApprovalStage(payload: CreateApprovalStage) {
    return (
      await api.post<ResponseModel<boolean>>(
        '/api/Approval/CreateApprovalStages',
        payload,
        {
          headers: RequestOptions(),
        }
      )
    ).data;
  }

  static async DeleteApprovalStage(payload: DeleteApprovalStages) {
    return (
      await api.delete<ResponseModel<boolean>>(
        '/api/Approval/DeleteRequestStages',
        {
          headers: RequestOptions(),
          data: payload,
        }
      )
    ).data;
  }
}
