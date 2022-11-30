<script setup lang="ts">
import ContainerComponent from '../components/ContainerComponent.vue';
import { reactive } from 'vue';
import { Login } from '../models/user';
import { useAuthStore } from 'src/stores/auth.store';

const login: Login = reactive({
  username: '',
  password: '',
  grant_type: 'password',
});

const emit = defineEmits(['loggedin']);

const submit = () => {
  let store = useAuthStore();
  store
    .login(login)
    .then(() => {
      emit('loggedin');
    })
    .catch((e) => console.log(e));
};

// export default defineComponent({
//   components: { ContainerComponent },
//   name: 'LoginPage',
//   setup() {
//     const router = useRouter();

//     const login: Login = reactive({
//       username: '',
//       password: '',
//       grant_type: 'password',
//     });

//     const submit = () => {
//       let store = useAuthStore();
//       store
//         .login(login)
//         .then(() => {
//           router.push('/');
//         })
//         .catch((e) => console.log(e));
//     };
//     return { login, submit };
//   },
// });
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

        <q-input
          class="q-mt-sm"
          outlined
          v-model="login.grant_type"
          label="Grant Type"
        />

        <div class="q-mt-md">
          <q-btn class="q-ml-sm" color="positive" type="submit"> Login </q-btn>
        </div>
      </form>
    </ContainerComponent>
  </q-page>
</template>
