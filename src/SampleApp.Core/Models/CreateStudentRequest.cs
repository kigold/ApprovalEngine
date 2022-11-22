namespace SampleApp.Core.Models
{
    public record CreateStudentRequest(string firstName, string lastName, string email);
    public record UpdateStudentRequest(long studentId, string firstName, string lastName);
}
