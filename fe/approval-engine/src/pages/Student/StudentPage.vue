<template>
  <q-page padding>
    <StudentTable
      :students="getStudents"
      :loading="getLoading"
      :pagination="pagination"
      @request="handleRequest"
    ></StudentTable>
    <q-page-sticky position="bottom-right" :offset="[18, 18]">
      <q-btn @click="createNewStudent" fab icon="add" color="accent" />
    </q-page-sticky>
  </q-page>
</template>

<script setup lang="ts">
import StudentTable from './StudentTable.vue';
import { GetStudentsQuery } from 'src/models/student';
import { QBtn, QPage, QPageSticky } from 'quasar';
import { computed, onMounted } from 'vue';
import { useRouter } from 'vue-router';
import { useStudentStore } from 'src/stores/student.store';
import { PaginationModel } from 'src/models/response.model';

const router = useRouter();
const store = useStudentStore();
const emit = defineEmits(['triggerNegative']);

let pageQuery: GetStudentsQuery = {
  pageIndex: 1,
  pageSize: 0,
};

const getStudents = computed(() => {
  return store.getStudents;
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

const error = computed(() => {
  return store.getError;
});

onMounted(() => {
  store.fetchStudentsAsync(pageQuery).finally(() => {
    if (error.value) emit('triggerNegative');
  });
});

const handleRequest = (pagination: PaginationModel) => {
  pageQuery.pageIndex = pagination.page as number;
  pageQuery.pageSize = pagination.rowsPerPage as number;
  store.fetchStudentsAsync(pageQuery);
};

function createNewStudent() {
  router.push('student/create');
}
</script>
