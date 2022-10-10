using SampleApp.Core.Models;

namespace SampleApp.Core.Services
{
    public interface IStudentService
    {
        Task<StudentResponse> GetStudent(long id);
        Task<IEnumerable<StudentResponse>> GetAllStudents();
        Task CreateStudent(CreateStudentRequest model);
        Task UpdateStudent(UpdateStudentRequest model);
        Task DeleteStudent(long id);
    }
}
