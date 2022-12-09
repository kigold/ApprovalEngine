<script setup lang="ts">
import ContainerComponent from '../components/ContainerComponent.vue';
import { reactive } from 'vue';
import { Login } from '../models/user';
import { useAuthStore } from 'src/stores/auth.store';
import { useRouter } from 'vue-router';

const store = useAuthStore();
const router = useRouter();

const login: Login = reactive({
  username: '',
  password: '',
});

if (store.getUserProfile) router.push('/');

const emit = defineEmits(['loggedin']);

const submit = () => {
  store
    .login(login)
    .then(() => {
      emit('loggedin');
    })
    .catch((e) => console.log(e));
};
</script>

<template>
  <q-page padding>
    <ContainerComponent>
      <h3>Login</h3>
      <form @submit.prevent="submit">
        <q-input
          class="q-mt-sm"
          outlined
          v-model="login.username"
          label="Username"
        />

        <q-input
          class="q-mt-sm"
          outlined
          v-model="login.password"
          label="Password"
          type="password"
        />

        <div class="q-mt-md">
          <q-btn class="q-ml-sm" color="positive" type="submit"> Login </q-btn>
        </div>
      </form>
    </ContainerComponent>
  </q-page>
</template>
