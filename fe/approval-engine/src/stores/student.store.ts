import { defineStore } from 'pinia';
import { Student, GetStudentsQuery, CreateStudent } from 'src/models/student';
import StudentService from '../services/student.service';
import config from 'src/config';
import { Toast } from 'src/services/toast.helper';

export const useStudentStore = defineStore('student', {
  state: () => ({
    students: <Student[]>[],
    student: <Student>{},
    totalCount: 0,
    page: 1,
    pageSize: 0,
    loading: false,
    error: '',
  }),
  getters: {
    getStudents: (state) => {
      return state.students;
    },
    getStudent: (state) => {
      return state.student;
    },
    getTotalCount: (state) => {
      return state.totalCount;
    },
    getLoading: (state) => {
      return state.loading;
    },
    getPage: (state) => {
      return state.page;
    },
    getError: (state) => {
      return state.page;
    },
  },
  actions: {
    async fetchStudentsAsync(payload: GetStudentsQuery) {
      payload.pageSize =
        payload.pageSize === 0 ? config.pageSize : payload.pageSize;
      this.loading = true;
      try {
        const response = await StudentService.GetStudents(payload);
        if (!response.hasErrors) {
          this.students = response.payload;
          this.totalCount = response.totalCount;
          this.pageSize = payload.pageSize;
          this.page = payload.pageIndex;
        } else {
          this.error = response.errors.toString();
        }
      } catch (error: any) {
        console.log('Caught error from API', error.message);
        this.error = error.message as string;
        Toast(error.message);
      } finally {
        this.loading = false;
      }
    },

    async fetchStudentAsync(id: number) {
      this.loading = true;
      try {
        const response = await StudentService.GetStudent(id);
        if (!response.hasErrors) {
          this.student = response.payload;
        } else {
          this.error = response.errors.toString();
        }
      } catch (error: any) {
        this.error = error as string;
        Toast(error.message);
      } finally {
        this.loading = false;
      }
    },

    async createStudentAsync(payload: CreateStudent) {
      this.loading = true;
      try {
        const response = await StudentService.CreateStudent(payload);
        if (response.hasErrors) {
          this.error = response.errors.toString();
        }
      } catch (error: any) {
        this.error = error as string;
        Toast(error.message);
      } finally {
        this.loading = false;
      }
    },
  },
});
