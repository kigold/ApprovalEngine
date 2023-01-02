<template>
  <div class="q-pa-md" style="max-width: 350px">
    List:
    <div>{{ tempList }}</div>
    <div>
      <div
        class="drag-el drop-zone"
        draggable="true"
        v-for="(item, index) in tempList"
        v-bind:key="item.id"
        @dragstart="startDrag($event, index)"
        @drop="onDrop($event, index)"
        @dragover.prevent
        @dragenter.prevent
      >
        <q-item-section> {{ index }} ||| {{ item.name }} </q-item-section>
      </div>
      DraggedItem:
      <div>{{ draggedIndex }}</div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { QSplitter, QTabs, QTab, QTabPanels, QTabPanel } from 'quasar';
import { ref } from 'vue';

let tempList = ref([
  { id: 1, name: 'item1' },
  { id: 2, name: 'item2' },
  { id: 3, name: 'item3' },
]);
//const tempList = ref([...list.value]);
let draggedIndex = ref();

const startDrag = (event: DragEvent, index: number) => {
  console.log('list\n', tempList.value.map((x) => x.name).join('-\n'));
  console.log('dragging item index', index);
  draggedIndex.value = index;
  console.log('dragged item index', draggedIndex);
  // if (event?.dataTransfer) {
  //   event.dataTransfer.dropEffect = 'move';
  //   event.dataTransfer.effectAllowed = 'move';
  //   event.dataTransfer.setData('itemId', item.id);
  // }
};

const onDrop = (event: DragEvent, index: number) => {
  event.preventDefault;
  console.log('dropping item index', index);
  swapItem(tempList.value, draggedIndex.value, index);
};

const swapItem = (list: any, dragged: number, dropped: number) => {
  console.log('swapping', dragged, dropped);
  console.log('swapping', list[dragged].name, list[dropped].name);
  let temp = list[dragged];
  list[dragged] = list[dropped];
  //console.log('temp', temp.name);
  list[dropped] = temp;
  console.log('list after swap/n', list.map((x) => x.name).join('-\n'));
};
</script>

<style>
.drop-zosne {
  width: 50%;
  margin: 50px auto;
  background-color: bisque;
  padding: 10px;
  min-height: 10px;
}
.drag-el {
  background-color: rgb(31, 114, 165);
  color: white;
  padding: 5px;
  margin-bottom: 10px;
}
</style>
