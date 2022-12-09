import { api } from 'boot/axios';
import { ResponseModel } from 'src/models/response.model';
import { CreateStudent, GetStudentsQuery, Student } from 'src/models/student';

const RequestOptions = () => {
  return {
    Accept: '*/*',
    //'Content-Type': 'application/json;charset=utf-8',
    Authorization: `Bearer ${localStorage.getItem('token')}`,
  };
};

export default class StudentService {
  static async GetStudents(payload: GetStudentsQuery) {
    console.log('calling APi Page ooooo: ', payload);
    return (
      await api.get<ResponseModel<Student[]>>('/api/Student', {
        params: { pageIndex: payload.pageIndex, pageSize: payload.pageSize },
        headers: RequestOptions(),
      })
    ).data;
  }

  static async GetStudent(id: number) {
    return (
      await api.get<ResponseModel<Student>>(`/api/Student/${id}`, {
        headers: RequestOptions(),
      })
    ).data;
  }

  static async CreateStudent(payload: CreateStudent) {
    return (
      await api.post<ResponseModel<boolean>>('/api/Student', payload, {
        headers: RequestOptions(),
      })
    ).data;
  }
}
