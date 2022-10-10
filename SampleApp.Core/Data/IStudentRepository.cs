using SampleApp.Core.Data.Entities;

namespace SampleApp.Core.Data
{
    public interface IStudentRepository : IDisposable
    {
        IEnumerable<Student> GetStudents();
        Task<Student> GetStudentByIdAsync(long studentId);
        Task InsertStudentAsync(Student student);
        Task DeleteStudentAsync(long studentID);
        Task UpdateStudentAsync(Student student);
        Task SaveAsync();
    }
}
