using ApprovalEngine;
using ApprovalEngine.Enums;
using ApprovalEngine.Models;
using Moq;
using SampleApp.Core;
using SampleApp.Core.Data.Entities.ApprovalEngine;
using System.Collections;
using UnitTests.ApprovalEngineTest.Mock;

namespace UnitTests.ApprovalEngineTest
{
    public class ApprovalEngineTests
    {
        private IApprovalService _approvalService;

        public ApprovalEngineTests()
        {
            _approvalService = new ApprovalService(
                    MockApprovalRepository.GetMock().Object,
                    MockApprovalStageRepository.GetMock().Object,
                    MockApprovalHistoryRepository.GetMock().Object,
                    new Mock<IHttpUserService>().Object
                );
        }

        #region Get Tests

        public static IEnumerable<object[]> GetApprovalRequestByPermissionSetupData()
        {
            yield return new object[]
            {
                new GetApprovalsRequestByPermission
                {
                    Permission = Permission.HOD
                },
                MockApprovalRepository.ApprovalRequests.Where(x => MockApprovalStageRepository.ApprovalStages.Any(s => s.Permission == Permission.HOD && s.ApprovalType == x.ApprovalType && x.Version == s.Version && x.StageOrder == s.StageOrder)).ToList()
            };
            yield return new object[]
             {
                new GetApprovalsRequestByPermission
                {
                    Permission = Permission.Admin
                },
                MockApprovalRepository.ApprovalRequests.Where(x => MockApprovalStageRepository.ApprovalStages.Any(s => s.Permission == Permission.Admin && s.ApprovalType == x.ApprovalType && x.Version == s.Version && x.StageOrder == s.StageOrder)).ToList()
};
        }

        [Theory]
        [MemberData(nameof(GetApprovalRequestByPermissionSetupData))]
        public async Task GetRequestsByPermission_ReturnsCorrectResponse(GetApprovalsRequestByPermission request, List<ApprovalRequest> expectedResponse)
        {
            //Arrange

            //Act
            var result = await _approvalService.GetRequestsByPermission(request);

            //Assert
            Assert.Equal(expectedResponse.Count, result.Data.Items.Count());
            foreach (var approval in expectedResponse)
            {
                var approvalResult = result.Data.Items.FirstOrDefault(x => x.RequestId == approval.Id);
                Assert.Equal(approval.Stage, approvalResult.Stage);
                Assert.Equal(approval.Status.ToString(), approvalResult.Status);
                Assert.Equal(approval.ApprovalType.ToString(), approvalResult.ApprovalType);
                Assert.Equal(approval.StageOrder, approvalResult.StatgeOrder);
                Assert.Equal(approval.EntityId, approvalResult.EntityId);
            }
        }

        public static IEnumerable<object[]> GetApprovalRequestSetupData()
        {
            yield return new object[]
            {
                new GetApprovalsRequest
                {
                    EntityId = "3"
                },
                MockApprovalRepository.ApprovalRequests.Where(x => x.Id == 3).ToList()
            };
            yield return new object[]
            {
                new GetApprovalsRequest
                {
                    ApprovalRequestType = ApprovalType.StudentUser,
                    IsPending = true,
                },
                MockApprovalRepository.ApprovalRequests.Where(x => x.ApprovalType == ApprovalType.StudentUser && (x.Status == ApprovalStatus.Created || x.Status == ApprovalStatus.Pending)).ToList()
            };
            yield return new object[]
            {
                new GetApprovalsRequest
                {
                    ApprovalRequestType = ApprovalType.StudentUser,
                    IsPending = false,
                },
                MockApprovalRepository.ApprovalRequests.Where(x => x.Id == 1 && x.ApprovalType == ApprovalType.StudentUser && (x.Status == ApprovalStatus.Approved || x.Status == ApprovalStatus.Rejected
                    || x.Status == ApprovalStatus.Returned)).ToList()
            };
            yield return new object[]
            {
                new GetApprovalsRequest
                {
                    ApprovalRequestType = ApprovalType.StudentUser,
                },
                MockApprovalRepository.ApprovalRequests.Where(x => x.ApprovalType == ApprovalType.StudentUser).ToList()
            };
            yield return new object[]
            {
                new GetApprovalsRequest
                {
                    ApprovalRequestType = ApprovalType.AdminUser
                },
                MockApprovalRepository.ApprovalRequests.Where(x => x.ApprovalType == ApprovalType.AdminUser).ToList()
            };
        }

        [Theory]
        [MemberData(nameof(GetApprovalRequestSetupData))]
        public async Task GetRequests_ReturnsCorrectResponse(GetApprovalsRequest request, List<ApprovalRequest> expectedResponse)
        {
            //Arrange

            //Act
            var result = await _approvalService.GetRequests(request);

            //Assert
            Assert.Equal(expectedResponse.Count, result.Data.Items.Count());
            foreach (var approval in expectedResponse)
            {
                var approvalResult = result.Data.Items.FirstOrDefault(x => x.RequestId == approval.Id);
                Assert.Equal(approval.Stage, approvalResult.Stage);
                Assert.Equal(approval.Status.ToString(), approvalResult.Status);
                Assert.Equal(approval.ApprovalType.ToString(), approvalResult.ApprovalType);
                Assert.Equal(approval.StageOrder, approvalResult.StatgeOrder);
                Assert.Equal(approval.EntityId, approvalResult.EntityId);
            }
        }

        [Theory]
        [InlineData(false, 1)]
        [InlineData(true, 5)]
        public async Task GetRequest_ReturnsCorrectResponse(bool hasError, long approvalRequestId)
        {
            //Arrange

            //Act
            var result = await _approvalService.GetRequest(approvalRequestId);

            //Assert
            Assert.NotNull(result);
            if (hasError)
            {
                Assert.True(result.HasError);
                Assert.Equal("Request not found.", result.ErrorMessages.First());
            }
            else
            {
                var expectedRequest = MockApprovalRepository.ApprovalRequests.FirstOrDefault(x => x.Id == approvalRequestId);
                Assert.Equal(expectedRequest.Stage, result.Data.Stage);
                Assert.Equal(expectedRequest.StageOrder, result.Data.StatgeOrder);
                Assert.Equal(expectedRequest.Status.ToString(), result.Data.Status);
                Assert.Equal(expectedRequest.ApprovalType.ToString(), result.Data.ApprovalType);
                Assert.False(result.HasError);
            }
        }
        #endregion

        #region Action Tests

        [Fact]
        public async Task GivenExisitingStage_CreateApprovalRequest_Successfully()
        {
            //Arrange
            var request = new CreateApprovalRequest(ApprovalType.StudentUser,"1", "");

            //Act
            var result = await _approvalService.CreateApprovalRequest(request);

            //Assert
            Assert.NotNull(result);
            Assert.False(result.HasError);
            Assert.True(result.Data);
        }

        [Fact]
        public async Task GivenNoExisitingStage_CreateApprovalRequest_ReturnError()
        {
            //Arrange
            var request = new CreateApprovalRequest(ApprovalType.Teacher, "1", "");

            //Act
            var result = await _approvalService.CreateApprovalRequest(request);

            //Assert
            Assert.NotNull(result);
            Assert.True(result.HasError);
            Assert.Equal("Approval Stages are not configured.", result.ErrorMessages.First());
        }

        [Theory]
        [InlineData(false, 1)]
        [InlineData(true, 5)]
        public async Task ApproveRequest_ReturnsCorrectResponse(bool hasError, long approvalRequestId)
        {
            //Arrange

            //Act
            var result = await _approvalService.ApproveRequest(new ApprovalModel(approvalRequestId, "HOD", "comment"));

            //Assert
            Assert.NotNull(result);
            if (hasError)
            {
                Assert.True(result.HasError);
                Assert.Equal($"Approval Request with Id {approvalRequestId} not found.", result.ErrorMessages.First());
            }
            else
            {
                Assert.True(result.Data);
                Assert.False(result.HasError);
            }
        }

        [Fact]
        public async Task ApproveRequest_MoveRequestToNextStage()
        {
            //Get Approval Request
            long approvalRequestId = 1;

            var approvalRequestsResult = await _approvalService.GetRequest(approvalRequestId);

            //Assert ApprovalRequest
            var expectedRequest = MockApprovalRepository.ApprovalRequests.FirstOrDefault(x => x.Id == approvalRequestId);
            Assert.Equal(expectedRequest.Stage, approvalRequestsResult.Data.Stage);
            Assert.Equal(expectedRequest.StageOrder, approvalRequestsResult.Data.StatgeOrder);
            Assert.Equal(expectedRequest.Status.ToString(), approvalRequestsResult.Data.Status);
            Assert.Equal(expectedRequest.ApprovalType.ToString(), approvalRequestsResult.Data.ApprovalType);

            //1st Approval
            var approvalResponse = await _approvalService.ApproveRequest(new ApprovalModel(approvalRequestId, "HOD", "comment"));
            Assert.True(approvalResponse.Data);
            Assert.False(approvalResponse.HasError);

            //Assert ApprovalRequest
            approvalRequestsResult = await _approvalService.GetRequest(approvalRequestId);
            Assert.Equal("Admin", approvalRequestsResult.Data.Stage);
            Assert.Equal(2, approvalRequestsResult.Data.StatgeOrder);
            Assert.Equal(ApprovalStatus.Pending.ToString(), approvalRequestsResult.Data.Status);
            Assert.Equal(expectedRequest.ApprovalType.ToString(), approvalRequestsResult.Data.ApprovalType);

            //2nd Approval
            approvalResponse = await _approvalService.ApproveRequest(new ApprovalModel(approvalRequestId, "Admin", "comment"));

            Assert.True(approvalResponse.Data);
            Assert.False(approvalResponse.HasError);

            //Assert ApprovalRequest
            approvalRequestsResult = await _approvalService.GetRequest(approvalRequestId);
            Assert.Equal("IT", approvalRequestsResult.Data.Stage);
            Assert.Equal(3, approvalRequestsResult.Data.StatgeOrder);
            Assert.Equal(ApprovalStatus.Pending.ToString(), approvalRequestsResult.Data.Status);
            Assert.Equal(expectedRequest.ApprovalType.ToString(), approvalRequestsResult.Data.ApprovalType);

            //3rd Approval
            approvalResponse = await _approvalService.ApproveRequest(new ApprovalModel(approvalRequestId, "IT", "comment"));

            Assert.True(approvalResponse.Data);
            Assert.False(approvalResponse.HasError);

            //Assert ApprovalRequest
            approvalRequestsResult = await _approvalService.GetRequest(approvalRequestId);
            Assert.Equal("Approver", approvalRequestsResult.Data.Stage);
            Assert.Equal(4, approvalRequestsResult.Data.StatgeOrder);
            Assert.Equal(ApprovalStatus.Pending.ToString(), approvalRequestsResult.Data.Status);
            Assert.Equal(expectedRequest.ApprovalType.ToString(), approvalRequestsResult.Data.ApprovalType);

            //4th Approval
            approvalResponse = await _approvalService.ApproveRequest(new ApprovalModel(approvalRequestId, "Approver", "comment"));

            Assert.True(approvalResponse.Data);
            Assert.False(approvalResponse.HasError);

            //Assert ApprovalRequest
            approvalRequestsResult = await _approvalService.GetRequest(approvalRequestId);
            Assert.Equal("Approver", approvalRequestsResult.Data.Stage);
            Assert.Equal(4, approvalRequestsResult.Data.StatgeOrder);
            Assert.Equal(ApprovalStatus.Approved.ToString(), approvalRequestsResult.Data.Status);
            Assert.Equal(expectedRequest.ApprovalType.ToString(), approvalRequestsResult.Data.ApprovalType);

        }

        [Fact]
        public async Task ApproveAndDeclineRequest_MoveRequestToPreviousStage()
        {
            //Get Approval Request
            long approvalRequestId = 1;

            var approvalRequestsResult = await _approvalService.GetRequest(approvalRequestId);

            //Assert ApprovalRequest
            var expectedRequest = MockApprovalRepository.ApprovalRequests.FirstOrDefault(x => x.Id == approvalRequestId);
            Assert.Equal(expectedRequest.Stage, approvalRequestsResult.Data.Stage);
            Assert.Equal(expectedRequest.StageOrder, approvalRequestsResult.Data.StatgeOrder);
            Assert.Equal(expectedRequest.Status.ToString(), approvalRequestsResult.Data.Status);
            Assert.Equal(expectedRequest.ApprovalType.ToString(), approvalRequestsResult.Data.ApprovalType);

            //1st Approval
            var approvalResponse = await _approvalService.ApproveRequest(new ApprovalModel(approvalRequestId, "HOD" , "comment"));
            Assert.True(approvalResponse.Data);
            Assert.False(approvalResponse.HasError);

            //Assert ApprovalRequest
            approvalRequestsResult = await _approvalService.GetRequest(approvalRequestId);
            Assert.Equal("Admin", approvalRequestsResult.Data.Stage);
            Assert.Equal(2, approvalRequestsResult.Data.StatgeOrder);
            Assert.Equal(ApprovalStatus.Pending.ToString(), approvalRequestsResult.Data.Status);
            Assert.Equal(expectedRequest.ApprovalType.ToString(), approvalRequestsResult.Data.ApprovalType);

            //Decline
            approvalResponse = await _approvalService.DeclineRequest(new ApprovalModel(approvalRequestId, "Admin" , "comment"));
            Assert.True(approvalResponse.Data);
            Assert.False(approvalResponse.HasError);

            //Assert ApprovalRequest
            approvalRequestsResult = await _approvalService.GetRequest(approvalRequestId);
            Assert.Equal("HOD", approvalRequestsResult.Data.Stage);
            Assert.Equal(1, approvalRequestsResult.Data.StatgeOrder);
            Assert.Equal(ApprovalStatus.Pending.ToString(), approvalRequestsResult.Data.Status);
            Assert.Equal(expectedRequest.ApprovalType.ToString(), approvalRequestsResult.Data.ApprovalType);

            //Decline
            approvalResponse = await _approvalService.DeclineRequest(new ApprovalModel(approvalRequestId, "HOD" , "comment"));
            Assert.True(approvalResponse.Data);
            Assert.False(approvalResponse.HasError);

            //Assert ApprovalRequest
            approvalRequestsResult = await _approvalService.GetRequest(approvalRequestId);
            Assert.Equal("HOD", approvalRequestsResult.Data.Stage);
            Assert.Equal(1, approvalRequestsResult.Data.StatgeOrder);
            Assert.Equal(ApprovalStatus.Rejected.ToString(), approvalRequestsResult.Data.Status);
            Assert.Equal(expectedRequest.ApprovalType.ToString(), approvalRequestsResult.Data.ApprovalType);
        }

        [Fact]
        public async Task ApproveAndRejectRequest_MoveRequestToPreviousStage()
        {
            //Get Approval Request
            long approvalRequestId = 1;

            var approvalRequestsResult = await _approvalService.GetRequest(approvalRequestId);

            //Assert ApprovalRequest
            var expectedRequest = MockApprovalRepository.ApprovalRequests.FirstOrDefault(x => x.Id == approvalRequestId);
            Assert.Equal(expectedRequest.Stage, approvalRequestsResult.Data.Stage);
            Assert.Equal(expectedRequest.StageOrder, approvalRequestsResult.Data.StatgeOrder);
            Assert.Equal(expectedRequest.Status.ToString(), approvalRequestsResult.Data.Status);
            Assert.Equal(expectedRequest.ApprovalType.ToString(), approvalRequestsResult.Data.ApprovalType);

            //1st Approval
            var approvalResponse = await _approvalService.ApproveRequest(new ApprovalModel(approvalRequestId, "HOD", "comment"));
            Assert.True(approvalResponse.Data);
            Assert.False(approvalResponse.HasError);

            //Assert ApprovalRequest
            approvalRequestsResult = await _approvalService.GetRequest(approvalRequestId);
            Assert.Equal("Admin", approvalRequestsResult.Data.Stage);
            Assert.Equal(2, approvalRequestsResult.Data.StatgeOrder);
            Assert.Equal(ApprovalStatus.Pending.ToString(), approvalRequestsResult.Data.Status);
            Assert.Equal(expectedRequest.ApprovalType.ToString(), approvalRequestsResult.Data.ApprovalType);

            //2nd Approval
            approvalResponse = await _approvalService.ApproveRequest(new ApprovalModel(approvalRequestId, "Admin", "comment"));

            Assert.True(approvalResponse.Data);
            Assert.False(approvalResponse.HasError);

            //Assert ApprovalRequest
            approvalRequestsResult = await _approvalService.GetRequest(approvalRequestId);
            Assert.Equal("IT", approvalRequestsResult.Data.Stage);
            Assert.Equal(3, approvalRequestsResult.Data.StatgeOrder);
            Assert.Equal(ApprovalStatus.Pending.ToString(), approvalRequestsResult.Data.Status);
            Assert.Equal(expectedRequest.ApprovalType.ToString(), approvalRequestsResult.Data.ApprovalType);

            //Reject
            approvalResponse = await _approvalService.RejectRequest(new ApprovalModel(approvalRequestId, "it" , "comment"));
            Assert.True(approvalResponse.Data);
            Assert.False(approvalResponse.HasError);

            //Assert ApprovalRequest
            approvalRequestsResult = await _approvalService.GetRequest(approvalRequestId);
            Assert.Equal("IT", approvalRequestsResult.Data.Stage);
            Assert.Equal(3, approvalRequestsResult.Data.StatgeOrder);
            Assert.Equal(ApprovalStatus.Rejected.ToString(), approvalRequestsResult.Data.Status);
            Assert.Equal(expectedRequest.ApprovalType.ToString(), approvalRequestsResult.Data.ApprovalType);
        }

        #endregion

        #region Admin Tests

        public static IEnumerable<object[]> GetApprovalStagesSetupData()
        {
            yield return new object[]
            {
                new GetApprovalStageRequest(1, ApprovalType.StudentUser),
                MockApprovalStageRepository.ApprovalStages.Where(x => x.ApprovalType == ApprovalType.StudentUser && x.Version == 1).ToList()
            };
            yield return new object[]
            {
                new GetApprovalStageRequest(2, ApprovalType.StudentUser),
                MockApprovalStageRepository.ApprovalStages.Where(x => x.ApprovalType == ApprovalType.StudentUser && x.Version == 2).ToList()
            };

            yield return new object[]
            {
                new GetApprovalStageRequest(1, ApprovalType.AdminUser),
                MockApprovalStageRepository.ApprovalStages.Where(x => x.ApprovalType == ApprovalType.AdminUser && x.Version == 1).ToList()
            };
        }

        [Theory]
        [MemberData(nameof(GetApprovalStagesSetupData))]
        public async Task GetApprovalStages_ReturnsCorrectResponse(GetApprovalStageRequest request, List<ApprovalStage> expectedResponse)
        {
            //Arrange
            //Act
            var result = await _approvalService.GetApprovalStages(request);

            //Assert
            Assert.Equal(expectedResponse.Count, result.Data.Count());
            foreach (var stage in expectedResponse)
            {
                var stageResult = result.Data.FirstOrDefault(x => x.ApprovalType == stage.ApprovalType.ToString() && x.Version == stage.Version && x.Order == stage.StageOrder);
                Assert.Equal(stage.Name, stageResult.Stage);
                Assert.Equal(stage.ApprovalType.ToString(), stageResult.ApprovalType);
                Assert.Equal(stage.StageOrder, stageResult.Order);
                Assert.Equal(stage.DeclineToOrder, stageResult.DeclineToOrder);
            }
        }

        [Fact]
        public async Task CreateApprovalStages_Successfully()
        {
            //Arrange
            var request = new CreateApprovalStage(
                    ApprovalType.StudentUser,
                    MockApprovalStageRepository.ApprovalStages.Where(x => x.Version == 1)
                    .Select(s => new ApprovalStageModel(s.Permission, "HOD", s.StageOrder, s.DeclineToOrder))
                    .ToList()
            );

            //Act
            var result = await _approvalService.CreateRequestStages(request);

            //Assert
            Assert.False(result.HasError);
            Assert.True(result.Data);
        }

        [Fact]
        public async Task CreateApprovalStages_InvalidData_ReturnError()
        {
            //Arrange
            var request = new CreateApprovalStage(
                    ApprovalType.StudentUser,
                    new List<ApprovalStageModel>
                    {
                        new ApprovalStageModel(Permission.HOD, "HOD", 1, 2),
                        new ApprovalStageModel(Permission.HOD, "HOD", 2, 1)
                    }
            );

            //Act
            var result = await _approvalService.CreateRequestStages(request);

            //Assert
            Assert.True(result.HasError);
            Assert.Equal("Decline To Order cannot be less than Order", result.ErrorMessages.First());
        }

        [Fact]
        public async Task DeleteApprovalStages_Successfully()
        {
            //Arrange
            var request = new DeleteApprovalStages(ApprovalType.StudentUser, 2);

            //Act
            var result = await _approvalService.DeleteRequestStages(request);

            //Assert
            Assert.False(result.HasError);
            Assert.True(result.Data);
        }

        [Theory]
        [InlineData(ApprovalType.Teacher, 1, "Approval Stages not found")]
        [InlineData(ApprovalType.StudentUser, 1, "The selected Approval Stages have pending approval requests and cannot be deleted")]
        public async Task DeleteApprovalStages_ReturnError(ApprovalType approvalType, int version, string expectedError)
        {
            //Arrange
            var request = new DeleteApprovalStages(approvalType, version);

            //Act
            var result = await _approvalService.DeleteRequestStages(request);

            //Assert
            Assert.True(result.HasError);
            Assert.Equal(expectedError, result.ErrorMessages.First());


            #endregion
        }
    }
}