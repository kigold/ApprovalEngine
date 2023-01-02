import { defineStore } from 'pinia';
import {
  Approval,
  ApprovalHistory,
  ApprovalModel,
  GetApprovalsQuery,
} from 'src/models/approval';
import ApprovalService from '../services/approval.service';
import config from 'src/config';
import { Toast } from 'src/services/toast.helper';

export const useApprovalStore = defineStore('approval', {
  state: () => ({
    approvals: <Approval[]>[],
    approvalHistory: <ApprovalHistory[]>[],
    approval: <Approval>{},
    totalCount: 0,
    page: 1,
    pageSize: 0,
    loading: false,
    error: '',
  }),
  getters: {
    getApprovals: (state) => {
      return state.approvals;
    },
    getApprovalHistory: (state) => {
      return state.approvalHistory;
    },
    getApproval: (state) => {
      return state.approval;
    },
    getTotalCount: (state) => {
      return state.totalCount;
    },
    getLoading: (state) => {
      return true;
    },
    getPage: (state) => {
      return state.page;
    },
    getError: (state) => {
      return state.page;
    },
  },
  actions: {
    async fetchApprovalsAsync(payload: GetApprovalsQuery) {
      payload.pageSize =
        payload.pageSize === 0 ? config.pageSize : payload.pageSize;
      this.loading = true;
      try {
        const response = await ApprovalService.GetApprovals(payload);
        if (!response.hasErrors) {
          this.approvals = response.payload;
          this.totalCount = response.totalCount;
          this.pageSize = payload.pageSize;
          this.page = payload.pageIndex;
        } else {
          this.error = response.errors.toString();
        }
      } catch (error: any) {
        this.error = error.message as string;
        Toast(error.message);
      } finally {
        this.loading = false;
      }
    },

    async fetchApprovalAsync(id: number) {
      this.loading = true;
      try {
        const response = await ApprovalService.GetApproval(id);
        if (!response.hasErrors) {
          this.approval = response.payload;
        } else {
          this.error = response.errors.toString();
        }
      } catch (error) {
        this.error = error as string;
      } finally {
        this.loading = false;
      }
    },

    async fetchApprovalHistoryAsync(id: number) {
      this.loading = true;
      try {
        const response = await ApprovalService.GetRequestHistory(id);
        if (!response.hasErrors) {
          this.approvalHistory = response.payload;
        } else {
          this.error = response.errors.toString();
        }
      } catch (error: any) {
        this.error = error.message as string;
        Toast(error.message);
      } finally {
        this.loading = false;
      }
    },

    async ApproveApprovalAsync(payload: ApprovalModel) {
      this.loading = true;
      try {
        const response = await ApprovalService.ApproveRequest(payload);
        if (response.hasErrors) {
          this.error = response.errors.toString();
        }
      } catch (error: any) {
        this.error = error as string;
        Toast(error.message);
      } finally {
        this.loading = false;
      }
    },

    async DeclineApprovalAsync(payload: ApprovalModel) {
      this.loading = true;
      try {
        const response = await ApprovalService.DeclineRequest(payload);
        if (response.hasErrors) {
          this.error = response.errors.toString();
        }
      } catch (error: any) {
        this.error = error as string;
        Toast(error.message);
      } finally {
        this.loading = false;
      }
    },

    async RejectApprovalAsync(payload: ApprovalModel) {
      this.loading = true;
      try {
        const response = await ApprovalService.RejectRequest(payload);
        if (response.hasErrors) {
          this.error = response.errors.toString();
        }
      } catch (error: any) {
        this.error = error as string;
        Toast(error.message);
      } finally {
        this.loading = false;
      }
    },

    async ReturnApprovalAsync(payload: ApprovalModel) {
      this.loading = true;
      try {
        const response = await ApprovalService.ReturnRequest(payload);
        if (response.hasErrors) {
          this.error = response.errors.toString();
        }
      } catch (error: any) {
        this.error = error as string;
        Toast(error.message);
      } finally {
        this.loading = false;
      }
    },

    async UpdateToPendingApprovalAsync(id: number) {
      this.loading = true;
      try {
        const response = await ApprovalService.UpdateToPending(id);
        if (response.hasErrors) {
          this.error = response.errors.toString();
        }
      } catch (error: any) {
        this.error = error as string;
        Toast(error.message);
      } finally {
        this.loading = false;
      }
    },
  },
});
