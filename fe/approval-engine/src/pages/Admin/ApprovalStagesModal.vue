<template>
  <q-dialog
    v-model="dialog"
    :maximized="maximizedToggle"
    transition-show="slide-up"
    transition-hide="slide-down"
  >
    <q-card class="column full-height full-width">
      <q-bar>
        <q-space />
        <q-btn dense flat icon="close" v-close-popup @click="toggle">
          <q-tooltip class="bg-white text-primary">Close</q-tooltip>
        </q-btn>
      </q-bar>

      <q-card-section>
        <div class="text-h6">Modify Approval Stages</div>
        <q-btn
          @click="triggerNewStageItemDialog"
          icon="add"
          color="primary"
          size="sm"
        />
      </q-card-section>

      <q-card-section class="q-pt-none">
        <q-list padding bordered separator>
          <q-item
            class="row"
            v-for="(stage, index) in list?.stages"
            v-bind:key="stage.order"
            draggable="true"
            @dragstart="startDrag($event, index)"
            @drop="onDrop($event, index)"
            @dragover.prevent
            @dragenter.prevent
          >
            <q-field
              class="q-pa-xs col-2"
              filled
              label="Order"
              stack-label
              readonly
              dense
            >
              <template v-slot:control>
                <div tabindex="0">
                  {{ stage.order }}
                </div>
              </template>
            </q-field>
            <q-field
              class="q-pa-xs col-2"
              filled
              label="Approval Type"
              stack-label
              readonly
              dense
            >
              <template v-slot:control>
                <div tabindex="0">
                  {{ stage.approvalType }}
                </div>
              </template>
            </q-field>
            <q-field
              class="q-pa-xs col-2"
              filled
              label="Permission"
              readonly
              stack-label
              dense
            >
              <template v-slot:control>
                <div tabindex="1">
                  {{ stage.permission }}
                </div>
              </template>
            </q-field>
            <q-field
              class="q-pa-xs col-2"
              filled
              label="Stage"
              stack-label
              dense
            >
              <template v-slot:control>
                <div tabindex="2">
                  {{ stage.stage }}
                </div>
              </template>
            </q-field>
            <q-input
              class="q-pa-xs col-2"
              filled
              label="Decline To"
              stack-label
              dense
              v-model="stage.declineToOrder"
            >
            </q-input>
            <q-btn
              icon="edit"
              flat
              class="q-pa-xs col-1"
              @click="editApprovalStage(index)"
            />
            <q-btn
              icon="delete"
              flat
              class="q-pa-xs col-1"
              @click="removeApprovalStage(index)"
            />
          </q-item>
        </q-list>
        <q-card-section>
          <q-btn
            color="primary"
            glossy
            label="Save"
            class="full-width"
            @click="saveApprovalStages"
          />
        </q-card-section>
      </q-card-section>
    </q-card>
  </q-dialog>

  <q-dialog
    v-model="newStageItemDialog"
    transition-show="scale"
    transition-hide="scale"
  >
    <q-card class="bg-primary text-white" style="width: 300px">
      <q-card-section>
        <div class="text-h6">Add Stage Item</div>
      </q-card-section>
      <q-card-section class="bg-white text-teal">
        <div class="q-pt-sm">
          <div class="q-gutter-y-md column" style="max-width: 300px">
            <q-input
              filled
              class="full-width"
              v-model="selectedStageItemData.approvalType"
              label="Approval Type"
              stack-label
              readonly
              dense
            />
          </div>
        </div>
        <div class="q-pt-sm">
          <div class="q-gutter-y-md column" style="max-width: 300px">
            <q-input
              filled
              v-model="selectedStageItemData.stage"
              label="Stage"
              stack-label
              dense
            />
          </div>
        </div>
        <div class="q-pt-sm">
          <div class="q-gutter-y-md column" style="max-width: 300px">
            <q-input
              filled
              v-model="selectedStageItemData.permission"
              label="Permission"
              stack-label
              dense
            />
          </div>
        </div>
        <div class="q-pt-sm">
          <div class="q-gutter-y-md column" style="max-width: 300px">
            <q-input
              filled
              v-model="selectedStageItemData.declineToOrder"
              label="Decline To Order"
              stack-label
              dense
            />
          </div>
        </div>
        <div class="q-pt-sm">
          <div class="q-gutter-y-md column" style="max-width: 300px">
            <q-input
              filled
              v-model="selectedStageItemData.order"
              label="Order"
              stack-label
              dense
            />
          </div>
        </div>
        <q-card-section>
          <q-btn
            color="primary"
            glossy
            label="Save"
            class="full-width"
            v-close-popup
            @click="saveStageItem(selectedStageItemData.order - 1)"
          />
        </q-card-section>
      </q-card-section>
    </q-card>
  </q-dialog>
</template>

<script setup lang="ts">
import {
  QDialog,
  QCard,
  QBar,
  QSpace,
  QBtn,
  QTooltip,
  QCardSection,
  QItem,
  QList,
} from 'quasar';
import { useApprovalStagesStore } from 'src/stores/approval-stages.store';
import { computed, PropType, reactive, ref, toRef, toRefs, version } from 'vue';
import {
  ApprovalStageByVersion,
  ApprovalStageModel,
  ApprovalStageResponse,
  ApprovalType,
  Permission,
} from 'src/models/approval';

const store = useApprovalStagesStore();
const props = defineProps({
  dialog: Boolean,
  maximizedToggle: Boolean,
  version: {
    type: Object as PropType<ApprovalStageByVersion>,
    required: false,
  },
  selectApprovalType: String,
});

let list = toRef(props, 'version', {
  stages: [] as ApprovalStageResponse[],
  version: 0,
} as ApprovalStageByVersion);
const selectApprovalType = toRef(props, 'selectApprovalType', '');
const emit = defineEmits(['toggle', 'refresh']);
let newStageItemDialog = ref(false);
let selectedStageItemData = ref<ApprovalStageResponse>(
  {} as ApprovalStageResponse
);
let isNewApprovalStage = true;

const toggle = () => {
  console.log('Triggering close');
  maximizedToggle.value = !maximizedToggle.value;
  emit('toggle');
};

let dialog = toRef(props, 'dialog');
let maximizedToggle = toRef(props, 'maximizedToggle');
maximizedToggle.value = false;
let draggedIndex = ref();

const startDrag = (event: DragEvent, index: number) => {
  draggedIndex.value = index;
};

const onDrop = (event: DragEvent, index: number) => {
  event.preventDefault;
  swapItem(list.value?.stages, draggedIndex.value, index);
};

const swapItem = (list: any, dragged: number, dropped: number) => {
  let temp = list[dragged];
  list[dragged] = list[dropped];
  list[dropped] = temp;
};

const saveApprovalStages = () => {
  let stages: ApprovalStageModel[] = <ApprovalStageModel[]>(
    list.value?.stages.map((x, index) => ({
      permission: Permission[x.permission as keyof typeof Permission],
      stage: x.stage,
      order: index + 1,
      declineToOrder: x.declineToOrder,
    }))
  );

  store.CreateApprovalStagesAsync({
    approvalRequestType:
      ApprovalType[
        list.value.stages[0].approvalType as keyof typeof ApprovalType
      ],
    stages: stages,
  });
  toggle();
  emit('refresh', list.value?.stages[0].approvalType);
};

const editApprovalStage = (index: number) => {
  isNewApprovalStage = false;

  const selectedItem = list.value?.stages[index];
  selectedStageItemData.value = {
    permission: selectedItem.permission,
    approvalType: selectedItem.approvalType,
    declineToOrder: selectedItem.declineToOrder,
    order: selectedItem.order,
    stage: selectedItem.stage,
    version: selectedItem.version + 1,
  };

  newStageItemDialog.value = true;
};

const triggerNewStageItemDialog = () => {
  isNewApprovalStage = true;

  if (list.value.stages && list.value.stages.length != 0) {
    const lastItem = list.value.stages[list.value.stages.length - 1];

    selectedStageItemData.value = {
      permission: '',
      approvalType: lastItem.approvalType,
      declineToOrder: lastItem.order,
      order: lastItem.order + 1,
      stage: '',
      version: lastItem.version + 1,
    };
  } else {
    selectedStageItemData.value = {
      permission: '',
      approvalType: selectApprovalType.value,
      declineToOrder: 0,
      order: 0,
      stage: '',
      version: 1,
    };
  }

  newStageItemDialog.value = true;
};

const saveStageItem = (index: number) => {
  if (isNewApprovalStage) {
    list.value.stages.push(selectedStageItemData.value);
  } else {
    list.value.stages[index] = selectedStageItemData.value;
  }
};

const removeApprovalStage = (index: number) => {
  list.value.stages.splice(index, 1);
};
</script>
