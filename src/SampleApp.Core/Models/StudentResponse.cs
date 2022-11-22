using SampleApp.Core.Data.Entities;

namespace SampleApp.Core.Models
{
    public class StudentResponse
    {
        public long Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public DateTime Created { get; set; }
        public DateTime? Modified { get; set; }
        public string Status { get; set; }

        public static implicit operator StudentResponse(Student model)
        {
            return model == null ? null : new StudentResponse
            {
                Id = model.Id,
                FirstName = model.FirstName,
                Email = model.Email,
                LastName = model.LastName,
                Created = model.Created,
                Modified = model.Modified,
                Status = model.Status.ToString()
            };
        }
    }
}
