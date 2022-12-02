<script setup lang="ts">
import ContainerComponent from '../../components/ContainerComponent.vue';
import { reactive } from 'vue';
import { useRouter } from 'vue-router';
import { CreateStudent } from 'src/models/student';
import { useStudentStore } from 'src/stores/student.store';

const router = useRouter();
const store = useStudentStore();
const emit = defineEmits(['notify']);

const student: CreateStudent = reactive({
  firstName: '',
  lastName: '',
  email: '',
});

const submit = async () => {
  await store.createStudentAsync(student);
  emit('notify', { type: 'info', message: 'Student Created' });
  router.push('/student');
};
</script>

<template>
  <q-page padding>
    <ContainerComponent>
      <h3>New Student</h3>
      <form @submit.prevent="submit">
        <q-input
          class="q-mt-sm"
          outlined
          v-model="student.firstName"
          label="First Name"
          lazy-rules
          :rules="[(val) => (val && val.length > 0) || 'Please type something']"
        />

        <q-input
          class="q-mt-sm"
          outlined
          v-model="student.lastName"
          label="Last Name"
          lazy-rules
          :rules="[(val) => (val && val.length > 0) || 'Please type something']"
        />

        <q-input
          class="q-mt-sm"
          outlined
          v-model="student.email"
          label="Email"
          dense
          lazy-rules
          :rules="[(val) => (val && val.length > 0) || 'Please type something']"
        />

        <div class="q-mt-md">
          <q-btn color="grey" to="/" type="reset">Cancel</q-btn>
          <q-btn class="q-ml-sm" color="positive" type="submit"> Create </q-btn>
        </div>
      </form>
    </ContainerComponent>
  </q-page>
</template>
