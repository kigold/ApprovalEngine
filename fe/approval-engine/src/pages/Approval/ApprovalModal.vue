<template>
  <q-dialog v-model="card">
    <q-card class="my-card" flat bordered>
      <q-card-section>
        <div class="text-overline text-orange-9">Approval</div>
        <q-field label="Id" stack-label dense>
          <template v-slot:control>
            <div class="self-center full-width" tabindex="0">
              {{ approval.requestId }}
            </div>
          </template>
        </q-field>
        <q-field label="Stage" stack-label dense>
          <template v-slot:control>
            <div class="self-center full-width" tabindex="0">
              {{ approval.stage }}
            </div>
          </template>
        </q-field>
        <q-field label="Status" stack-label dense>
          <template v-slot:control>
            <div class="self-center full-width" tabindex="0">
              {{ approval.status }}
            </div>
          </template>
        </q-field>
        <q-field label="Description" stack-label dense>
          <template v-slot:control>
            <div class="self-center full-width" tabindex="0">
              {{ approval.description }}
            </div>
          </template>
        </q-field>
        <q-input
          ref="inputRef"
          outlined
          v-model="comment"
          label="Approval Comment"
          dense
          :rules="[(val) => !!val || 'Comment is required']"
          v-show="showApprovalButton(approval.status)"
        />
      </q-card-section>

      <q-card-actions>
        <q-btn
          flat
          color="green"
          label="Approve"
          @click="approve"
          v-show="showApprovalButton(approval.status)"
        />
        <q-btn
          flat
          color="orange"
          label="Decline"
          @click="decline"
          v-show="showApprovalButton(approval.status)"
        />
        <q-btn
          flat
          color="red"
          label="Reject"
          @click="reject"
          v-show="showApprovalButton(approval.status)"
        />
        <q-btn color="grey" label="Cancel" @click="closeApprovalModal" />

        <q-space />

        <q-btn
          color="grey"
          round
          flat
          dense
          :icon="expanded ? 'keyboard_arrow_up' : 'keyboard_arrow_down'"
          @click="getApprovalHistory"
        >
          <q-tooltip> View Approval History </q-tooltip>
        </q-btn>
      </q-card-actions>

      <q-slide-transition>
        <div v-show="expanded">
          <q-separator />
          <q-card-section class="text-subitle2">
            <ApprovalHistoryComponent
              :approval="approval"
              :approval-history="approvalHistory"
            />
          </q-card-section>
        </div>
      </q-slide-transition>
      <q-inner-loading
        :showing="loading"
        label="Please wait..."
        label-class="text-teal"
        label-style="font-size: 1.1em"
      />
    </q-card>
  </q-dialog>
</template>

<script setup lang="ts">
import {
  QPage,
  QDialog,
  QCard,
  QImg,
  QCardSection,
  QBtn,
  QIcon,
  QRating,
  QSeparator,
  QCardActions,
  QTableProps,
  QInput,
} from 'quasar';
import {
  Approval,
  ApprovalHistory,
  ApprovalModel,
  GetApprovalsQuery,
} from 'src/models/approval';
import { PropType, ref, toRef } from 'vue';
import ApprovalHistoryComponent from 'src/pages/Approval/ApprovalHistoryComponent.vue';
import { useApprovalStore } from 'src/stores/approval.store';
import { computed } from 'vue';

const props = defineProps({
  approval: { type: Object as PropType<Approval>, required: true },
  card: Boolean,
  loading: Boolean,
});
const emit = defineEmits(['toggleCard', 'refreshApprovals']);
const store = useApprovalStore();
const card = toRef(props, 'card');
const approval = toRef(props, 'approval');
const loading = ref(false);
const expanded = ref(false);
const approvalHistory = computed(() => {
  return store.getApprovalHistory;
});
let hasApprovalHistory = ref(false);
let comment = ref('');
const inputRef = ref({} as QInput);
const approvalPayload = computed<ApprovalModel>(() => {
  return {
    approvalRequestId: approval.value.requestId,
    comment: comment.value,
    stage: approval.value.stage,
  };
});

const validateApprovalComment = async (
  approval: (payload: ApprovalModel) => Promise<void>,
  msg: string
) => {
  console.log(msg);
  if (!inputRef.value.validate()) {
    console.log(`Failed ${msg}`);
    return;
  }
  await approval(approvalPayload.value);

  console.log('Successful Approval');
  inputRef.value.resetValidation();
  comment.value = '';
  closeApprovalModal();
  emit('refreshApprovals');
};

const approve = async () => {
  validateApprovalComment(store.ApproveApprovalAsync, 'Approval');
};
const decline = () => {
  validateApprovalComment(store.DeclineApprovalAsync, 'Declining');
};
const reject = () => {
  validateApprovalComment(store.RejectApprovalAsync, 'Rejecting');
};
const closeApprovalModal = () => {
  console.log('closing card');
  expanded.value = false;
  hasApprovalHistory.value = false;
  emit('toggleCard');
};

const showApprovalButton = (status: string) => {
  if (status !== 'Created' && status !== 'Pending') return false;
  return true;
};

const getApprovalHistory = async () => {
  console.log('Fetching Approval History');
  expanded.value = !expanded.value;
  if (!hasApprovalHistory.value) {
    loading.value = true;
    await store.fetchApprovalHistoryAsync(approval.value.requestId);
    loading.value = false;
    hasApprovalHistory.value = true;
  }
};
</script>
