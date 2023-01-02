<template>
  <ApprovalStagesModal
    :dialog="createStageDialog"
    :is-new-approval-stages="isNewApprovalStages"
    :version="approvalVersion"
    :selectApprovalType="selectApprovalType"
    @toggle="toggleDialog"
    @refresh="refresh"
  />
  <q-page padding>
    <div class="q-pa-md" style="max-width: 800px">
      <div class="row">
        <div class="col-9 q-table__title">Approval Types</div>
        <q-btn
          size="sm"
          @click="createApprovalVersion(selectApprovalType, 1, true)"
          flat
          class="col-3 square"
          icon="add"
          label="Create New Stages"
        />
      </div>
      <q-list bordered separator>
        <q-expansion-item
          v-for="item in approvalTypes"
          v-bind:key="item.name"
          clickable
          icon="approval"
          :label="item.name"
          :caption="item.versionCount.toString() + ' versions'"
          @click="getApprovalTypeStages(item.name)"
          group="approvals"
        >
          <q-card>
            <q-list bordered separator>
              <q-item
                v-for="stage in approvalStages"
                v-bind:key="stage.version"
                draggable
              >
                <q-item-section class="q-pl-lg">
                  <q-item-label caption>
                    <div class="q-pa-md">
                      <q-table
                        :title="'Version ' + stage.version"
                        :rows="stage.stages"
                        :columns="stagesColumns"
                        row-key="name"
                        hide-bottom
                        dark
                      >
                        <template v-slot:top>
                          <div class="col-1 q-table__sm">
                            {{ 'Version ' + stage.version }}
                          </div>
                          <q-space />
                          <div class="q-gutter-lg row">
                            <q-btn
                              size="sm"
                              @click="
                                createApprovalVersion(
                                  stage.stages[0].approvalType,
                                  stage.version
                                )
                              "
                              flat
                              class="square"
                              icon="add"
                              label="Create"
                            />
                            <q-btn
                              size="sm"
                              @click="
                                deleteApprovalVersion(
                                  stage.stages[0].approvalType,
                                  stage.version
                                )
                              "
                              flat
                              class="square"
                              icon="delete"
                              label="Delete"
                            />
                          </div>
                        </template>

                        <template v-slot:body="props">
                          <q-tr :props="props">
                            <q-td key="Stage" :props="props">
                              <q-badge color="green">
                                {{ props.row.stage }}
                              </q-badge>
                            </q-td>
                            <q-td key="Permission" :props="props">
                              {{ props.row.permission }}
                            </q-td>
                            <q-td key="Order" :props="props">
                              {{ props.row.order }}
                            </q-td>
                            <q-td key="DeclineToOrder" :props="props">
                              {{ props.row.declineToOrder }}
                            </q-td>
                          </q-tr>
                        </template>
                      </q-table>
                    </div>
                  </q-item-label>
                </q-item-section>
              </q-item>
            </q-list>
          </q-card>
        </q-expansion-item>
      </q-list>
      <q-inner-loading :showing="loading">
        <q-spinner-gears size="50px" color="primary" />
      </q-inner-loading>
    </div>
  </q-page>
</template>

<script setup lang="ts">
import { QCard, QExpansionItem, QList, QTableProps } from 'quasar';
import {
  ApprovalStageByVersion,
  ApprovalStageResponse,
  ApprovalType,
  DeleteApprovalStages,
} from 'src/models/approval';
import ApprovalStagesModal from './ApprovalStagesModal.vue';
import { onMounted, PropType } from 'vue';
import { useApprovalStagesStore } from 'src/stores/approval-stages.store';
import { computed, ref } from 'vue';

const store = useApprovalStagesStore();
const props = defineProps({
  approvalTypes: {
    type: Object as PropType<string[]>,
    required: true,
  },
  query: Object as PropType<ApprovalType>,
});
const approvalVersion = ref<ApprovalStageByVersion>({
  stages: [] as ApprovalStageResponse[],
  version: 0,
} as ApprovalStageByVersion);
const approvalTypes = computed(() => {
  return store.approvalTypes;
});
const loading = computed(() => {
  return store.loading;
});
const approvalStages = ref<ApprovalStageByVersion[]>(
  [] as ApprovalStageByVersion[]
);
let createStageDialog = ref(false);
let isNewApprovalStages = ref(false);
let selectApprovalType = ref<string>('');

const toggleDialog = () => {
  createStageDialog.value = !createStageDialog.value;
};

// const createApprovalVersion = (approvalType: string, version: number) => {
//   console.log('Creating Item', approvalType, version);
//   stageDialog.value = true;
// };

const createApprovalVersion = (
  approvalType: string,
  version: number,
  newStages = false
) => {
  if (newStages) {
    approvalVersion.value = {
      version: version,
      stages: [] as ApprovalStageResponse[],
    } as ApprovalStageByVersion;
    createStageDialog.value = true;
    isNewApprovalStages.value = true;
    return;
  }

  ///clone here
  const item = approvalStages.value.filter((x) => x.version === version)[0];
  approvalVersion.value = {
    version: item.version,
    stages: item.stages.map((x) => ({
      approvalType: x.approvalType,
      declineToOrder: x.declineToOrder,
      order: x.order,
      permission: x.permission,
      stage: x.stage,
      version: x.version,
    })),
  };
  isNewApprovalStages.value = false;
  createStageDialog.value = true;
};

const deleteApprovalVersion = async (approvalType: string, version: number) => {
  console.log('Deleting Item', approvalType, version);
  let payload: DeleteApprovalStages = {
    approvalRequestType:
      ApprovalType[approvalType as keyof typeof ApprovalType],
    version: version,
  };
  await store.DeleteApprovalStagesAsync(payload);

  await refresh(approvalType);
};

const getApprovalTypeStages = async (approvalType: string) => {
  const approvalTypeEnum: ApprovalType = <ApprovalType>(<unknown>approvalType);
  if (!store.hasApprovalTypeStages(approvalType)) {
    await store.fetchApprovalStagesAsync(approvalTypeEnum);
  }
  approvalStages.value = store.getApprovalStage(approvalType);
};

onMounted(async () => {
  // get initial data from server (1st page)
  await store.fetchAllApprovalTypesAsync();
});

const refresh = async (approvalType: string) => {
  console.log('refreshing for ', approvalType);
  const approvalTypeEnum: ApprovalType = <ApprovalType>(<unknown>approvalType);
  //await store.fetchAllApprovalTypesAsync();
  await store.fetchApprovalStagesAsync(approvalTypeEnum);
  await store.fetchAllApprovalTypesAsync();
  approvalStages.value = store.getApprovalStage(approvalType);
  console.log('refreshed', approvalStages.value);
};

const stagesColumns: QTableProps['columns'] = [
  {
    name: 'Stage',
    required: true,
    label: 'Stage',
    field: 'stage',
  },
  {
    name: 'Permission',
    required: true,
    label: 'Permission',
    field: 'permission',
  },
  {
    name: 'Order',
    required: true,
    label: 'Order',
    field: 'order',
  },
  {
    name: 'DeclineToOrder',
    required: true,
    label: 'Decline To Order',
    field: 'declineToOrder',
  },
];
</script>
