<template>
  <q-card>
    <q-card-section>
      <div class="text-h6 text-grey-8">
        Inline Actions
        <q-btn
          label="Export"
          class="float-right text-capitalize text-indigo-8 shadow-3"
          icon="person"
        />
      </div>
    </q-card-section>
    <q-card-section class="q-pa-none">
      <q-table
        :rows="data"
        hide-bottom
        title="Students"
        color="accent"
        :loading="loading"
        row-key="id"
        v-model:pagination="pagination"
        @request="request"
      >
        <template v-slot:body-cell-Name="props">
          <q-td :props="props">
            <q-item style="max-width: 420px">
              <q-item-section avatar>
                <q-avatar>
                  <img :src="props.row.avatar" />
                </q-avatar>
              </q-item-section>

              <q-item-section>
                <q-item-label>{{ props.row.name }}</q-item-label>
              </q-item-section>
            </q-item>
          </q-td>
        </template>
        <template v-slot:body-cell-Action="props">
          <q-td :props="props">
            <q-btn icon="edit" size="sm" flat dense />
            <q-btn icon="delete" size="sm" class="q-ml-sm" flat dense />
          </q-td>
        </template>
      </q-table>
    </q-card-section>
  </q-card>
</template>

<script setup lang="ts">
import { emit } from 'cluster';
import {
  QTableProps,
  QCard,
  QCardSection,
  QBtn,
  QTable,
  QTd,
  QItem,
  QItemSection,
  QAvatar,
  QItemLabel,
} from 'quasar';
import { ApprovalStageModel } from 'src/models/approval';
import { TableProps } from 'src/models/table';
import { PropType, toRef } from 'vue';

const props = defineProps<TableProps<ApprovalStageModel>>();

let data = toRef(props, 'data');
let pagination = toRef(props, 'pagination');

const emit = defineEmits(['request']);

const request = (props: any) => {
  emit('request', props.pagination);
};

const editItem = () => {
  console.log('Editing Item');
};

const deleteItem = () => {
  console.log('Deleting Item');
};
</script>
