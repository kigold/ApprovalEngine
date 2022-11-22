using ApprovalEngine.Enums;
using Moq;
using SampleApp.Core.Data.Entities.ApprovalEngine;
using SampleApp.Core.Data.Repositories;
using System.Linq.Expressions;

namespace UnitTests.ApprovalEngineTest.Mock
{
    internal class MockApprovalRepository
    {
        public static List<ApprovalRequest> ApprovalRequests => new List<ApprovalRequest>
        {
                new ApprovalRequest
                {
                    Id = 1,
                    ApprovalType = ApprovalType.StudentUser,
                    EntityId = "1",
                    Stage = "HOD",
                    StageOrder = 1,
                    Status = ApprovalStatus.Created,
                    Version = 1
                },
                new ApprovalRequest
                {
                    Id = 2,
                    ApprovalType = ApprovalType.AdminUser,
                    EntityId = "2",
                    Stage = "Admin",
                    StageOrder = 1,
                    Status = ApprovalStatus.Created,
                    Version = 1
                },
                new ApprovalRequest
                {
                    Id = 3,
                    ApprovalType = ApprovalType.StudentUser,
                    EntityId = "3",
                    Stage = "IT",
                    StageOrder = 2,
                    Status = ApprovalStatus.Pending,
                    Version = 1
                },
        };
        public static Mock<IRepository<ApprovalRequest>> GetMock()
        {
            var mock = new Mock<IRepository<ApprovalRequest>>();

            var approvals = ApprovalRequests.AsQueryable();

            mock.Setup(x => x.Get(
                It.IsAny<Expression<Func<ApprovalRequest, bool>>>(),
                It.IsAny<Func<IQueryable<ApprovalRequest>, IOrderedQueryable<ApprovalRequest>>>(), 
                It.IsAny<string>()))
                .Returns(() => approvals);

            mock.Setup(x => x.GetByID(It.IsAny<long>()))
                .Returns((long id) => approvals.FirstOrDefault(a => a.Id == id));


            return mock;
        }
    }
}
