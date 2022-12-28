import { defineStore } from 'pinia';
import {
  ApprovalStageByVersion,
  CreateApprovalStages,
  DeleteApprovalStages,
  ApprovalType,
  ApprovalTypeResponse,
} from 'src/models/approval';
import ApprovalService from 'src/services/approval.service';
import { errorObject, HandleError } from 'src/services/toast.helper';

export const useApprovalStagesStore = defineStore('approvalStages', {
  state: () => ({
    approvalTypes: <ApprovalTypeResponse[]>[],
    allStages: <Record<string, ApprovalStageByVersion[]>>{},
    stages: <ApprovalStageByVersion[]>[],
    version: <ApprovalStageByVersion>{},
    loading: false,
    error: <errorObject>{},
    approvalType: String,
    approvalType2: ApprovalType,
  }),
  getters: {
    getApprovalStage: (state) => {
      return (approvalType: string) => {
        return state.allStages[approvalType];
      };
    },

    getVersionStages: (state) => {
      return (approvalType: string, version: number) => {
        return state.allStages[approvalType].filter(
          (x) => x.version === version
        );
      };
    },

    hasApprovalTypeStages: (state) => {
      return (approvalType: string) => {
        return state.allStages[approvalType] != undefined;
      };
    },
  },
  actions: {
    async fetchAllApprovalTypesAsync() {
      this.loading = true;
      try {
        const response = await ApprovalService.GetAllApprovalTypes();
        if (!response.hasErrors) {
          this.approvalTypes = response.payload;
        } else {
          HandleError(response.errors.toString(), this.error);
        }
      } catch (error: any) {
        HandleError(error.message, this.error);
      } finally {
        this.loading = false;
      }
    },

    async fetchApprovalStagesAsync(payload: ApprovalType) {
      this.loading = true;
      try {
        const response = await ApprovalService.GetAllApprovalStages(payload);
        if (!response.hasErrors) {
          this.allStages[`${payload}`] = response.payload;
          this.stages = this.allStages[payload];
        } else {
          HandleError(response.errors.toString(), this.error);
        }
      } catch (error: any) {
        HandleError(error.message, this.error);
      } finally {
        this.loading = false;
      }
    },

    async CreateApprovalStagesAsync(payload: CreateApprovalStages) {
      this.loading = true;
      try {
        const response = await ApprovalService.CreateApprovalStage(payload);
        if (!response.hasErrors) {
        } else {
          HandleError(response.errors.toString(), this.error);
        }
      } catch (error: any) {
        HandleError(error.message, this.error);
      } finally {
        this.loading = false;
      }
    },

    async DeleteApprovalStagesAsync(payload: DeleteApprovalStages) {
      try {
        const response = await ApprovalService.DeleteApprovalStage(payload);
        if (!response.hasErrors) {
        } else {
          HandleError(response.errors.toString(), this.error);
        }
      } catch (error: any) {
        HandleError(error.message, this.error);
      } finally {
        this.loading = false;
      }
    },
  },
});
