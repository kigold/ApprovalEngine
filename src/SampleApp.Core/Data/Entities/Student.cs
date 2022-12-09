using SampleApp.Core.Enums;

namespace SampleApp.Core.Data.Entities
{
    public class Student
    {
        public long Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public DateTime Created { get; set; }
        public DateTime? Modified { get; set; }
        public StudentStatus Status { get; set; }
    }
}
