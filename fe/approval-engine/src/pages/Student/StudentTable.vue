<template>
  <q-page padding>
    <ContainerComponent>
      <div class="q-pa-md">
        <q-table
          ref="tableRef"
          title="Students"
          color="accent"
          :rows="students"
          :loading="loading"
          row-key="id"
          v-model:pagination="pagy"
          @request="request"
        >
        </q-table>
      </div>
    </ContainerComponent>
  </q-page>
</template>

<script setup lang="ts">
import { QPage, QTable, QTableProps } from 'quasar';
import { Student } from 'src/models/student';
import { PropType, ref, toRef } from 'vue';
import ContainerComponent from 'src/components/ContainerComponent.vue';

const props = defineProps({
  students: { type: Object as PropType<Student[]>, required: true },
  loading: Boolean,
  pagination: { type: Object as PropType<QTableProps['pagination']> },
  total: { type: Number },
});

const tableRef = ref();

let students = toRef(props, 'students');
let pagy = toRef(props, 'pagination');

// watch(props, () => {
//   pagy.value = props.pagination;
//   stu.value = props.students;
// });

const emit = defineEmits(['request']);

const request = (props: any) => {
  emit('request', props.pagination);
};

// onMounted(() => {
//   // get initial data from server (1st page)
//   tableRef.value.requestServerInteraction();
// });
</script>
