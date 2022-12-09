<template>
  <q-page padding>
    <ContainerComponent>
      Student Component
      <div class="q-pa-md">
        <q-table title="Treats" :rows="rows" :columns="columns" row-key="name">
          <template v-slot:top="props">
            <div class="col-2 q-table__title">Treats</div>

            <q-space />

            <div class="col">
              <q-toggle
                v-model="visibleColumns"
                val="calories"
                label="Calories"
              />
              <q-toggle v-model="visibleColumns" val="fat" label="Fat" />
              <q-toggle v-model="visibleColumns" val="carbs" label="Carbs" />
              <q-toggle
                v-model="visibleColumns"
                val="protein"
                label="Protein"
              />
              <q-toggle v-model="visibleColumns" val="sodium" label="Sodium" />
              <q-toggle
                v-model="visibleColumns"
                val="calcium"
                label="Calcium"
              />
              <q-toggle v-model="visibleColumns" val="iron" label="Iron" />
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
import { QPage, QTable, QSpace, QToggle, QBtn } from 'quasar';
import { ref } from 'vue';

const visibleColumns = ref(true);
</script>
