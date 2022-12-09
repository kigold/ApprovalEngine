<template>
  <ApprovalModal
    v-model:approval="approval"
    :card="approvalCard"
    @toggle-card="toggleApprovalCard"
    @refresh-approvals="query"
  />
  <q-page padding>
    <ContainerComponent>
      <div class="q-pa-md">
        <q-table
          ref="tableRef"
          title="Approvals"
          color="accent"
          :rows="approvals"
          :loading="loading"
          row-key="requestId"
          v-model:pagination="pagination"
          @request="request"
          selection="single"
          v-model:selected="selected"
          @row-click="onRowClick"
        >
          <template v-slot:top="props">
            <div class="col-2 q-table__title">Approvals</div>

            <q-space />

            <div class="q-gutter-md row">
              <q-toggle
                toggle-indeterminate
                v-model="pageQuery.isPending"
                val="isPending"
                label="is Pending?"
              />
              <q-toggle
                toggle-indeterminate
                v-model="pageQuery.isReturned"
                val="isReturned"
                label="is Returned?"
              />
              <q-separator vertical />
              <q-input
                class="row"
                outlined
                v-model="pageQuery.entityId"
                label="Entity Id"
                dense
              />
              <q-btn
                @click="query"
                flat
                class="square"
                color="primary"
                icon="send"
              />
            </div>

            <q-btn
              flat
              round
              dense
              :icon="props.inFullscreen ? 'fullscreen_exit' : 'fullscreen'"
              @click="props.toggleFullscreen"
              class="q-ml-md"
            />
          </template>
        </q-table>
      </div>
    </ContainerComponent>
  </q-page>
</template>

<script setup lang="ts">
import {
  QBtn,
  QInput,
  QPage,
  QSeparator,
  QSpace,
  QTable,
  QTableProps,
  QToggle,
} from 'quasar';
import { Approval } from 'src/models/approval';
import { PropType, reactive, ref, toRef } from 'vue';
import { GetApprovalsQuery } from 'src/models/approval';
import ContainerComponent from 'src/components/ContainerComponent.vue';
import { useRouter } from 'vue-router';
import ApprovalModal from './ApprovalModal.vue';

const router = useRouter();

const props = defineProps({
  approvals: { type: Object as PropType<Approval[]>, required: true },
  loading: Boolean,
  pagination: { type: Object as PropType<QTableProps['pagination']> },
  total: { type: Number },
  pageQuery: { type: Object as PropType<GetApprovalsQuery>, required: true },
});

const tableRef = ref();

let approvals = toRef(props, 'approvals');
let pagination = toRef(props, 'pagination');
let pageQuery = toRef(props, 'pageQuery');
let selected = ref([] as Approval[]);
let approval = ref<Approval>({} as Approval);
let approvalCard = ref(false);

//const isPending = ref(null);
//const filter = reactive({ isPending: null, isReturned: null });

const emit = defineEmits(['request']);

const request = (requestProp: any) => {
  console.log('requesting ...', props.pagination);
  console.log('requesting ...', requestProp.pagination);
  emit('request', requestProp.pagination);
};

const query = () => {
  console.log('querying ...', pagination.value);
  emit('request', pagination.value);
};

const onRowClick = (evt: any, row: any) => {
  console.log('row clicked', row);
  approval.value = row;
  approvalCard.value = true;
  console.log(approvalCard.value);
};

const toggleApprovalCard = () => {
  console.log('toggling card', approvalCard.value);
  approvalCard.value = !approvalCard.value;
  console.log('toggled card', approvalCard.value);
};

// onMounted(() => {
//   // get initial data from server (1st page)
//   tableRef.value.requestServerInteraction();
// });
</script>
