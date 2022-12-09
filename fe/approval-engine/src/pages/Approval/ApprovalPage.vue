<template>
  <q-page padding>
    <ApprovalTable
      :approvals="getApprovals"
      :loading="getLoading"
      :pagination="pagination"
      :pageQuery="pageQuery"
      @request="handleRequest"
    ></ApprovalTable>
  </q-page>
</template>

<script setup lang="ts">
import ApprovalTable from 'src/pages/Approval/ApprovalTable.vue';
import { QBtn, QPage, QPageSticky } from 'quasar';
import { computed, onMounted, ref } from 'vue';
import { useApprovalStore } from 'src/stores/approval.store';
import { PaginationModel } from 'src/models/response.model';
import { GetApprovalsQuery } from 'src/models/approval';

const store = useApprovalStore();

// let pageQuery: GetApprovalsQuery = {
//   pageIndex: 1,
//   pageSize: 10,
//   ApprovalRequestType: 0,
//   entityId: undefined,
//   isPending: false,
//   isReturned: undefined,
// };

let pageQuery = ref<GetApprovalsQuery>({
  pageIndex: 1,
  pageSize: 10,
  ApprovalRequestType: 0,
  entityId: undefined,
  isPending: undefined,
  isReturned: undefined,
});

const getApprovals = computed(() => {
  return store.getApprovals;
});

const getLoading = computed(() => {
  return store.getLoading;
});

const pagination = computed(() => {
  return {
    rowsPerPage: store.pageSize,
    page: store.getPage,
    rowsNumber: store.getTotalCount,
  };
});

onMounted(() => {
  store.fetchApprovalsAsync(pageQuery.value);
});

const handleRequest = (pagination: PaginationModel) => {
  console.log('requesting', pagination.rowsPerPage, pagination);
  pageQuery.value.pageIndex = pagination.page as number;
  pageQuery.value.pageSize = pagination.rowsPerPage as number;
  store.fetchApprovalsAsync(pageQuery.value);
};
</script>
