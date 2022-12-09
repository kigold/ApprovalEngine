namespace SampleApp.Core.Data.Entities.ApprovalEngine
{
    public abstract class Entity
    {
        public long Id { get; set; }
        public DateTime Created { get; set; }
        public DateTime? Modified { get; set; }
        public long CreatedBy { get; set; }
        public User Creator { get; set; }
    }
}
