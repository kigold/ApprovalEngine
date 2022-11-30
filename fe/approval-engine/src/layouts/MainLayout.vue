<template>
  <q-layout view="hHh lpR fFf">
    <q-header elevated class="bg-primary text-white" height-hint="98">
      <q-toolbar>
        <q-btn dense flat round icon="menu" @click="toggleLeftDrawer" />

        <q-toolbar-title>
          <q-avatar>
            <img src="https://cdn.quasar.dev/logo-v2/svg/logo-mono-white.svg" />
          </q-avatar>
          Approval Engine
        </q-toolbar-title>

        <div v-show="loggedInFlag">
          <q-btn round>
            <q-avatar size="42px">
              <img src="https://cdn.quasar.dev/img/avatar2.jpg" />
            </q-avatar>
          </q-btn>
          <q-btn @click="logout" round color="primary" icon="directions">
            <q-tooltip>Logout</q-tooltip>
          </q-btn>
        </div>
      </q-toolbar>

      <q-tabs align="left">
        <q-route-tab to="/student" label="Students" />
        <q-route-tab to="/student/create" label="New Student" />
        <q-route-tab to="/approval" label="Approvals" />
      </q-tabs>
    </q-header>

    <q-drawer
      class="bg-secondary"
      show-if-above
      v-model="leftDrawerOpen"
      side="left"
      elevated
      :mini="miniState"
      @mouseover="miniState = false"
      @mouseout="miniState = true"
    >
      <!-- drawer content -->
      <q-list>
        <q-item
          active-class="tab-active"
          to="/"
          exact
          class="q-ma-none navigation-item"
          clickable
          v-ripple
        >
          <q-item-section avatar>
            <q-icon name="home" />
          </q-item-section>

          <q-item-section> Home </q-item-section>
        </q-item>
        <q-item
          active-class="tab-active"
          to="/student"
          exact
          class="q-ma-none navigation-item"
          clickable
          v-ripple
        >
          <q-item-section avatar>
            <q-icon name="dashboard" />
          </q-item-section>

          <q-item-section> Students </q-item-section>
        </q-item>
        <q-item
          active-class="tab-active"
          to="/approval"
          exact
          class="q-ma-none navigation-item"
          clickable
          v-ripple
        >
          <q-item-section avatar>
            <q-icon name="view_headline" />
          </q-item-section>

          <q-item-section> Approvals </q-item-section>
        </q-item>
      </q-list>
    </q-drawer>

    <q-page-container>
      <router-view @loggedin="afterLogin" />
    </q-page-container>

    <template>
      <div class="q-pa-md">
        <div class="row q-gutter-sm">
          <q-btn
            no-caps
            unelevated
            color="positive"
            @click="triggerPositive"
            label="Trigger 'positive'"
          />
          <q-btn
            no-caps
            unelevated
            color="negative"
            @click="triggerNegative"
            label="Trigger 'negative'"
          />
          <q-btn
            no-caps
            unelevated
            color="warning"
            text-color="dark"
            @click="triggerWarning"
            label="Trigger 'warning'"
          />
          <q-btn
            no-caps
            unelevated
            color="info"
            @click="triggerInfo"
            label="Trigger 'info'"
          />
          <q-btn
            no-caps
            unelevated
            color="grey-8"
            @click="triggerOngoing"
            label="Trigger 'ongoing'"
          />
        </div>
      </div>
    </template>

    <q-footer elevated class="bg-primary text-white">
      <q-toolbar>
        <q-toolbar-title>
          <q-avatar>
            <img src="https://cdn.quasar.dev/logo-v2/svg/logo-mono-white.svg" />
          </q-avatar>
          <div>Title</div>
        </q-toolbar-title>
      </q-toolbar>
    </q-footer>
  </q-layout>
</template>

<script setup lang="ts">
import { ref } from 'vue';
import { useRouter } from 'vue-router';
import { useQuasar } from 'quasar';

const $q = useQuasar();
function triggerNegative() {
  console.log('Triggering Negative message');
  $q.notify({
    type: 'Negative',
    message: 'Shit happened',
  });
}

const router = useRouter();
const leftDrawerOpen = ref(false);
const miniState = ref(true);

const loggedIn = localStorage.getItem('user') ? true : false;
let loggedInFlag = ref(loggedIn);

function toggleLeftDrawer() {
  leftDrawerOpen.value = !leftDrawerOpen.value;
}

function afterLogin() {
  loggedInFlag.value = true;
  router.push('/');
}

function logout() {
  localStorage.removeItem('user');
  localStorage.removeItem('token');
  loggedInFlag.value = false;
  router.push('/login');
}
</script>
