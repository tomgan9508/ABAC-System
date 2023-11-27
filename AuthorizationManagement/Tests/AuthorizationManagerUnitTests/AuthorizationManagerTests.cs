using AuthorizationManagement.ResourceManager;
using Common.Application;
using Common.Application.Clients;
using Common.Application.Dtos.Authorization;
using Common.Application.Models.API;
using Common.Cosmos.Models.Entities;
using Common.Cosmos.Models.Properties;
using Common.Cosmos.Providers;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace AuthorizationManagementTests.AuthorizationManagerUnitTests
{
    [TestClass]
    public class AuthorizationManagerTests
    {
        protected Mock<IUsersContainerProvider> UsersContainerProviderMock;
        protected Mock<IResourcesContainerProvider> ResourcesContainerProviderMock;
        protected Mock<IHttpClientExtension> HttpClientExtensionMock;
        protected Mock<ILogger<AuthorizationManager>> LoggerMock;


        protected IDictionary<string, AbacUserEntity> Users;
        protected IDictionary<string, UserApprovedPoliciesEntity> UserApprovedPolicies;
        protected IDictionary<string, AbacResourceEntity> Resources;
        protected AuthorizationManager AuthorizationManager;

        [TestInitialize]
        public void TestInitialize()
        {
            UsersContainerProviderMock = new Mock<IUsersContainerProvider>();
            ResourcesContainerProviderMock = new Mock<IResourcesContainerProvider>();
            HttpClientExtensionMock = new Mock<IHttpClientExtension>();
            LoggerMock = new Mock<ILogger<AuthorizationManager>>();

            Users = CreateUsers(1);
            UsersContainerProviderMock.Setup(m => m.GetUserInfoByPartition<AbacUserEntity>(It.IsAny<string>()))
                .ReturnsAsync((string userId) =>
                {
                    if (Users.ContainsKey(userId))
                    {
                        var responseMock = new Mock<FeedResponse<AbacUserEntity>>();
                        responseMock.Setup(x => x.Resource).Returns(new List<AbacUserEntity>() { Users[userId] });
                        return responseMock.Object;
                    }

                    return null;
                });

            UserApprovedPolicies = CreateEmptyUserApprovedPolicies(Users);
            UsersContainerProviderMock.Setup(m => m.GetUserInfoByPartition<UserApprovedPoliciesEntity>(It.IsAny<string>()))
                .ReturnsAsync((string userId) =>
                {
                    if (UserApprovedPolicies.ContainsKey(userId))
                    {
                        var responseMock = new Mock<FeedResponse<UserApprovedPoliciesEntity>>();
                        responseMock.Setup(x => x.Resource).Returns(new List<UserApprovedPoliciesEntity>() { UserApprovedPolicies[userId] });
                        return responseMock.Object;
                    }

                    return null;
                });

            Resources = CreateResources(1);
            ResourcesContainerProviderMock.Setup(m => m.GetResource(It.IsAny<string>()))
                .ReturnsAsync((string resourceId) =>
                {
                    if (Resources.ContainsKey(resourceId))
                    {
                        var responseMock = new Mock<ItemResponse<AbacResourceEntity>>();
                        responseMock.Setup(x => x.Resource).Returns(Resources[resourceId]);
                        return responseMock.Object;
                    }

                    return null;
                });

            UsersContainerProviderMock.Setup(m => m.AddUserApprovedPolicy(It.IsAny<string>(), It.IsAny<string>()));
            UsersContainerProviderMock.Setup(m => m.CreateOrUpdateUserApprovedPolicies(
                It.IsAny<string>(), It.IsAny<IDictionary<string, DateTime>>(), It.IsAny<bool>()))
                .ReturnsAsync((string userId, IDictionary<string, DateTime> approvedPolicies, bool returnResponse) =>
                {
                    var responseMock = new Mock<ItemResponse<UserApprovedPoliciesEntity>>();
                    responseMock.Setup(x => x.Resource).Returns(UserApprovedPolicies[userId]);
                    return responseMock.Object;
                });

            AuthorizationManager = new AuthorizationManager(UsersContainerProviderMock.Object,
                                                ResourcesContainerProviderMock.Object,
                                                HttpClientExtensionMock.Object,
                                                LoggerMock.Object);
        }

        protected IDictionary<string, AbacResourceEntity> CreateResources(int size)
        {
            var result = new Dictionary<string, AbacResourceEntity>();
            for (int i = 0; i < size; i++)
            {
                string resourceId = $"resourceId_{i}";
                result.Add(resourceId,
                           new AbacResourceEntity(new AbacResourceProperties(
                               new AbacResource(resourceId), SystemIdGenerator.GenerateId(resourceId))));
            }

            return result;
        }

        protected IDictionary<string, UserApprovedPoliciesEntity> CreateEmptyUserApprovedPolicies(IDictionary<string, AbacUserEntity> users)
        {
            var result = new Dictionary<string, UserApprovedPoliciesEntity>();
            foreach (KeyValuePair<string, AbacUserEntity> user in users)
            {
                result.Add(user.Key,
                           new UserApprovedPoliciesEntity(new UserApprovedPoliciesProperties(user.Key,
                               new Dictionary<string, DateTime>(),
                               SystemIdGenerator.GenerateId(user.Key))));
            }

            return result;
        }

        protected IDictionary<string, AbacUserEntity> CreateUsers(int size)
        {
            var result = new Dictionary<string, AbacUserEntity>();
            for (int i = 0; i < size; i++)
            {
                string userId = $"userId_{i}";
                result.Add(userId,
                           new AbacUserEntity(new AbacUserProperties(new AbacUser(userId), SystemIdGenerator.GenerateId(userId))));
            }

            return result;
        }

        [TestClass]
        public class IsAuthorizedAsyncUnitTests : AuthorizationManagerTests
        {
            [TestMethod]
            public async Task ShouldReturnNullWhenUserNotFound()
            {
                string missingUserId = "missingUserId";
                string resourceId = "resourceId_0";

                var response = await AuthorizationManager.IsAuthorizedAsync(missingUserId, resourceId);

                Assert.IsNull(response);
            }

            [TestMethod]
            public async Task ShouldReturnNullWhenResourceNotFound()
            {
                string missingUserId = "userId";
                string resourceId = "missingResourceId";

                var response = await AuthorizationManager.IsAuthorizedAsync(missingUserId, resourceId);

                Assert.IsNull(response);
            }

            [TestMethod]
            public async Task ShouldAuthorizeUserWhenNoUserApprovalFound()
            {
                var user = Users.First();
                var resource = Resources.First();
                resource.Value.Properties.Properties.Policies = new HashSet<string>()
                {
                    "policyId_1",
                    "policyId_2",
                    "policyId_3"
                };

                HttpClientExtensionMock.Setup(m => m.PostAsync<AuthorizationEngineResponse>(It.IsAny<Uri>(), It.IsAny<string>()))
                                       .ReturnsAsync(new AuthorizationEngineResponse()
                                       {
                                           ContainsFulfilledPolicy = true,
                                           PolicyFulfilledId = "policyId_1"
                                       });

                var response = await AuthorizationManager.IsAuthorizedAsync(user.Key, resource.Key);

                Assert.IsTrue(response.IsAuthorized);
                UsersContainerProviderMock.Verify(x => x.CreateOrUpdateUserApprovedPolicies(
                    It.IsAny<string>(), It.IsAny<Dictionary<string, DateTime>>(), It.IsAny<bool>()), Times.Never);
            }
            [TestMethod]
            public async Task ShouldNotAuthorizeUserWhenNoUserApprovalFound()
            {
                var user = Users.First();
                var resource = Resources.First();
                resource.Value.Properties.Properties.Policies = new HashSet<string>()
                {
                    "policyId_1",
                    "policyId_2",
                    "policyId_3"
                };

                HttpClientExtensionMock.Setup(m => m.PostAsync<AuthorizationEngineResponse>(It.IsAny<Uri>(), It.IsAny<string>()))
                                       .ReturnsAsync(new AuthorizationEngineResponse()
                                       {
                                           ContainsFulfilledPolicy = false,
                                       });

                var response = await AuthorizationManager.IsAuthorizedAsync(user.Key, resource.Key);

                Assert.IsFalse(response.IsAuthorized);
                UsersContainerProviderMock.Verify(x => x.CreateOrUpdateUserApprovedPolicies(
                    It.IsAny<string>(), It.IsAny<Dictionary<string, DateTime>>(), It.IsAny<bool>()), Times.Never);
            }

            [TestMethod]
            public async Task ShouldPreAuthorizeUserWhenUserApprovalsNotExpired()
            {
                var user = Users.First();
                var resource = Resources.First();
                resource.Value.Properties.Properties.Policies = new HashSet<string>()
                {
                    "policyId_1",
                    "policyId_2",
                    "policyId_3"
                };

                UserApprovedPolicies[user.Key] = new UserApprovedPoliciesEntity(
                    new UserApprovedPoliciesProperties(user.Key,
                    new Dictionary<string, DateTime>()
                        {
                            { "policyId_1", DateTime.UtcNow.AddDays(-1) }
                        },
                    SystemIdGenerator.GenerateId(user.Key)));

                HttpClientExtensionMock.Setup(m => m.PostAsync<PreAuthorizationEngineResponse>(It.IsAny<Uri>(), It.IsAny<string>()))
                                       .ReturnsAsync(new PreAuthorizationEngineResponse()
                                       {
                                           UnExpiredPolicyApprovals = new List<string>()
                                           {
                                               "policyId_1"
                                           },
                                           ExpiredPolicyApprovals = new List<string>()
                                       });

                var response = await AuthorizationManager.IsAuthorizedAsync(user.Key, resource.Key);

                Assert.IsTrue(response.IsAuthorized);
            }

            [TestMethod]
            public async Task ShouldRemoveUserExpiredApprovals()
            {
                var user = Users.First();
                var resource = Resources.First();

                resource.Value.Properties.Properties.Policies = new HashSet<string>()
                {
                    "policyId_1",
                    "policyId_2",
                    "policyId_3"
                };

                UserApprovedPolicies[user.Key] = new UserApprovedPoliciesEntity(
                    new UserApprovedPoliciesProperties(user.Key,
                    new Dictionary<string, DateTime>()
                        {
                            { "policyId_1", DateTime.UtcNow.AddDays(-1) },
                            { "policyId_2", DateTime.UtcNow.AddDays(-1) }
                        },
                    SystemIdGenerator.GenerateId(user.Key)));

                HttpClientExtensionMock.Setup(m => m.PostAsync<AuthorizationEngineResponse>(It.IsAny<Uri>(), It.IsAny<string>()))
                                       .ReturnsAsync(new AuthorizationEngineResponse());

                HttpClientExtensionMock.Setup(m => m.PostAsync<PreAuthorizationEngineResponse>(It.IsAny<Uri>(), It.IsAny<string>()))
                       .ReturnsAsync(new PreAuthorizationEngineResponse()
                       {
                           UnExpiredPolicyApprovals = new List<string>(),
                           ExpiredPolicyApprovals = new List<string>()
                            {
                                "policyId_1", "policyId_2"
                            },
                       });

                var response = await AuthorizationManager.IsAuthorizedAsync(user.Key, resource.Key);

                UsersContainerProviderMock.Verify(x => x.CreateOrUpdateUserApprovedPolicies(
                    It.IsAny<string>(), It.IsAny<Dictionary<string, DateTime>>(), It.IsAny<bool>()), Times.Once);
            }

            [TestMethod]
            public async Task ShouldAuthorizeUserWhenResourceIsPublic()
            {
                var user = Users.First();
                var resource = Resources.First();

                var response = await AuthorizationManager.IsAuthorizedAsync(user.Key, resource.Key);

                Assert.IsTrue(response.IsAuthorized);
            }

            [TestMethod]
            public async Task ShouldAuthorizeUserEvenWhenUserApprovalsExpired()
            {
                var user = Users.First();
                var resource = Resources.First();

                resource.Value.Properties.Properties.Policies = new HashSet<string>()
                {
                    "policyId_1",
                    "policyId_2",
                    "policyId_3"
                };

                UserApprovedPolicies[user.Key] = new UserApprovedPoliciesEntity(
                    new UserApprovedPoliciesProperties(user.Key,
                    new Dictionary<string, DateTime>()
                        {
                            { "policyId_1", DateTime.UtcNow.AddDays(-1) },
                            { "policyId_2", DateTime.UtcNow.AddDays(-1) }
                        },
                    SystemIdGenerator.GenerateId(user.Key)));

                HttpClientExtensionMock.Setup(m => m.PostAsync<AuthorizationEngineResponse>(It.IsAny<Uri>(), It.IsAny<string>()))
                                       .ReturnsAsync(new AuthorizationEngineResponse()
                                       {
                                           ContainsFulfilledPolicy = true,
                                           PolicyFulfilledId = "policyId_1"
                                       });

                HttpClientExtensionMock.Setup(m => m.PostAsync<PreAuthorizationEngineResponse>(It.IsAny<Uri>(), It.IsAny<string>()))
                       .ReturnsAsync(new PreAuthorizationEngineResponse()
                       {
                           UnExpiredPolicyApprovals = new List<string>(),
                           ExpiredPolicyApprovals = new List<string>()
                            {
                                "policyId_1", "policyId_2"
                            },
                       });

                var response = await AuthorizationManager.IsAuthorizedAsync(user.Key, resource.Key);

                Assert.IsTrue(response.IsAuthorized);
                UsersContainerProviderMock.Verify(x => x.CreateOrUpdateUserApprovedPolicies(
                    It.IsAny<string>(), It.IsAny<Dictionary<string, DateTime>>(), It.IsAny<bool>()), Times.Once);
            }
        }
    }
}
