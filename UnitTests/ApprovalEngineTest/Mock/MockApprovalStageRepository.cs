using ApprovalEngine.Enums;
using Moq;
using SampleApp.Core.Data.Entities.ApprovalEngine;
using SampleApp.Core.Data.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace UnitTests.ApprovalEngineTest.Mock
{
    internal class MockApprovalStageRepository
    {
        public static List<ApprovalStage> ApprovalStages => new List<ApprovalStage>
        {
                new ApprovalStage
                {
                    Id = 1,
                    ApprovalType = ApprovalType.StudentUser,
                    DeclineToOrder = 1,
                    StageOrder = 1,
                    Name = "HOD",
                    Permission = Permission.HOD,
                    Version = 1
                },
                new ApprovalStage
                {
                    Id = 2,
                    ApprovalType = ApprovalType.StudentUser,
                    DeclineToOrder = 1,
                    StageOrder = 2,
                    Name = "Admin",
                    Permission = Permission.Admin,
                    Version = 1
                },
                new ApprovalStage
                {
                    Id = 3,
                    ApprovalType = ApprovalType.StudentUser,
                    DeclineToOrder = 2,
                    StageOrder = 3,
                    Name = "IT",
                    Permission = Permission.IT,
                    Version = 1
                },
                new ApprovalStage
                {
                    Id = 4,
                    ApprovalType = ApprovalType.StudentUser,
                    DeclineToOrder = 3,
                    StageOrder = 4,
                    Name = "Approver",
                    Permission = Permission.Approver,
                    Version = 1
                },
                new ApprovalStage
                {
                    Id = 1,
                    ApprovalType = ApprovalType.StudentUser,
                    DeclineToOrder = 1,
                    StageOrder = 1,
                    Name = "HOD",
                    Permission = Permission.HOD,
                    Version = 2
                },
                new ApprovalStage
                {
                    Id = 2,
                    ApprovalType = ApprovalType.StudentUser,
                    DeclineToOrder = 1,
                    StageOrder = 2,
                    Name = "Admin",
                    Permission = Permission.Admin,
                    Version = 2
                },
                new ApprovalStage
                {
                    Id = 3,
                    ApprovalType = ApprovalType.StudentUser,
                    DeclineToOrder = 2,
                    StageOrder = 3,
                    Name = "IT",
                    Permission = Permission.IT,
                    Version = 2
                },
                new ApprovalStage
                {
                    Id = 4,
                    ApprovalType = ApprovalType.StudentUser,
                    DeclineToOrder = 3,
                    StageOrder = 4,
                    Name = "Approver",
                    Permission = Permission.Approver,
                    Version = 2
                }
        };

        public static Mock<IRepository<ApprovalStage>> GetMock()
        {
            var mock = new Mock<IRepository<ApprovalStage>>();

            var stages = ApprovalStages.AsQueryable();

            mock.Setup(x => x.Get(
                It.IsAny<Expression<Func<ApprovalStage, bool>>>(),
                It.IsAny<Func<IQueryable<ApprovalStage>, IOrderedQueryable<ApprovalStage>>>(),
                It.IsAny<string>()))
                .Returns(() => stages);

            return mock;
        }
    }
}
