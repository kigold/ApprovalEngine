<template>
  <div class="q-pa-md q-gutter-md">
    <q-list bordered class="rounded-borders" style="max-width: 500px">
      <q-item-label header>Approval History </q-item-label>

      <q-item v-for="item in approvalHistory" v-bind:key="item.dateTime">
        <q-item-section avatar>
          <q-avatar>
            <img src="https://cdn.quasar.dev/img/avatar2.jpg" />
          </q-avatar>
        </q-item-section>

        <q-item-section>
          <q-item-label class="text-primary" lines="1">{{
            item.action
          }}</q-item-label>
          <q-item-label caption>
            {{ item.comment }}
            <q-tooltip>
              {{ item.comment }}
            </q-tooltip>
          </q-item-label>
        </q-item-section>

        <q-item-section side top>
          <q-item-label caption>{{
            date.formatDate(item.dateTime, 'DD dddd MMMM hh:mm')
          }}</q-item-label>
          <q-tooltip>
            {{ date.formatDate(item.dateTime, 'YYYY-MM-DDTHH:mm:ss.SSSZ') }}
          </q-tooltip>
          <q-item-label class="text-weight-bold" caption>{{
            item.creator
          }}</q-item-label>
          <q-tooltip>
            {{ item.creator }}
          </q-tooltip>
        </q-item-section>
      </q-item>

      <q-separator inset="item" />
    </q-list>
  </div>
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
} from 'quasar';
import {
  Approval,
  ApprovalHistory,
  GetApprovalsQuery,
} from 'src/models/approval';
import { PropType, ref, toRef } from 'vue';
import { date } from 'quasar';

const props = defineProps({
  approvalHistory: {
    type: Object as PropType<ApprovalHistory[]>,
    required: true,
  },
  approval: { type: Object as PropType<Approval>, required: true },
  loading: Boolean,
});

//const emit = defineEmits(['toggleCard']);

//const approvalHistory = toRef(props, 'approval');
const expanded = ref(false);
const approvalHistory = toRef(props, 'approvalHistory');
</script>
