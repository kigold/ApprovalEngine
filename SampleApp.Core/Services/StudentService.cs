using SampleApp.Core.Data.Entities;
using SampleApp.Core.Data.Repositories;
using SampleApp.Core.Models;

namespace SampleApp.Core.Services
{
    public class StudentService : IStudentService
    {
        private readonly IStudentRepository _studentRepo;
        public StudentService(IStudentRepository studentRepo)
        {
            _studentRepo =  studentRepo;
        }

        public async Task CreateStudent(CreateStudentRequest model)
        {
            await _studentRepo.InsertStudentAsync(new Student
            {
                FirstName = model.firstName,
                LastName = model.lastName,
                Email = model.email,
                Created = DateTime.Now,
            });
        }

        public async Task DeleteStudent(long id)
        {
            await _studentRepo.DeleteStudentAsync(id);
        }

        public async Task<IEnumerable<StudentResponse>> GetAllStudents()
        {
            return _studentRepo.GetStudents().Select(x => (StudentResponse)x);
        }

        public async Task<StudentResponse> GetStudent(long id)
        {
            return await _studentRepo.GetStudentByIdAsync(id);
        }

        public async Task UpdateStudent(UpdateStudentRequest model)
        {
            var student = await _studentRepo.GetStudentByIdAsync(model.studentId);
            student.FirstName = model.firstName;
            student.LastName = model.lastName;
            student.Modified = DateTime.Now;
            await _studentRepo.UpdateStudentAsync(student);
        }
    }
}
