using SampleApp.Core.Data.Entities;

namespace SampleApp.Core.Data.Repositories
{
    public class StudentRepository : IStudentRepository, IDisposable
    {
        private SampleDbContext _context;
        private bool _disposed = false;
        public StudentRepository(SampleDbContext context)
        {
            _context = context;
        }

        public async Task<Student> GetStudentByIdAsync(long studentId)
        {
            return await _context.Students.FindAsync(studentId);
        }

        public IEnumerable<Student> GetStudents()
        {
            return _context.Students.ToList();
        }

        public async Task InsertStudentAsync(Student student)
        {
            await _context.Students.AddAsync(student);
            await _context.SaveChangesAsync();
        }

        public async Task SaveAsync()
        {
            await _context.SaveChangesAsync();
        }

        public async Task UpdateStudentAsync(Student student)
        {
            _context.Students.Update(student);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteStudentAsync(long studentId)
        {
            var student = _context.Students.Find(studentId);
            _context.Students.Remove(student);
            await _context.SaveChangesAsync();
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    _context.Dispose();
                }
            }
            _disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
