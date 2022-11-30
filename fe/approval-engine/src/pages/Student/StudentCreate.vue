<script setup lang="ts">
import ContainerComponent from '../../components/ContainerComponent.vue';
import { reactive } from 'vue';
import { useRouter } from 'vue-router';
//import Student from '../../models/student';
// export default defineComponent({
//   components: { ContainerComponent },
//   name: 'StudentNew',
//   setup() {
//     const router = useRouter();

//     const student = reactive({
//       firstName: '',
//       lastName: '',
//       email: '',
//     });

//     const submit = () => {
//       router.push('/');
//       student.email = 'tester';
//     };
//     return { student, submit };
//   },
// });
const router = useRouter();

const student = reactive({
  firstName: '',
  lastName: '',
  email: '',
});

const submit = () => {
  router.push('/student');
  student.email = 'tester';
};
</script>

<template>
  <q-page padding>
    <ContainerComponent>
      <h3>New Student</h3>
      <form @submit="submit">
        <q-input
          class="q-mt-sm"
          outlined
          v-model="student.firstName"
          label="First Name"
        />

        <q-input
          class="q-mt-sm"
          outlined
          v-model="student.lastName"
          label="Last Name"
        />

        <q-input
          class="q-mt-sm"
          outlined
          v-model="student.email"
          label="Email"
          dense
        />

        <div class="q-mt-md">
          <q-btn color="grey" to="/" type="reset">Cancel</q-btn>
          <q-btn class="q-ml-sm" color="positive" type="submit"> Create </q-btn>
        </div>
      </form>
    </ContainerComponent>
  </q-page>
</template>
