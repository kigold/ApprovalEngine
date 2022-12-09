using SampleApp.Core.Data.Entities.ApprovalEngine;
using Moq;
using SampleApp.Core.Data.Repositories;

namespace UnitTests.ApprovalEngineTest.Mock
{
    internal class MockApprovalHistoryRepository
    {
        public static Mock<IRepository<ApprovalHistory>> GetMock()
        {
            var mock = new Mock<IRepository<ApprovalHistory>>();

            return mock;
        }
    }
}
