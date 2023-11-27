using Common.Application;
using Common.Application.Dtos.Authorization;
using Common.Application.Models.API;
using Common.Cosmos.Models.Properties;
using PreAuthorization.Engines;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Tests.PreAuthorizationEngineUnitTests
{
    [TestClass]
    public class PreAuthorizationEngineTests
    {
        protected IList<PolicyProperties> ResourcePolicies;
        protected PreAuthorizationEngine PreAuthorizationEngine;

        [TestInitialize]
        public void TestInitialize()
        {
            ResourcePolicies = CreateResourcePolicies(3);
            var userIds = new List<string> { "user1", "user2", "user3" };

            PreAuthorizationEngine = new PreAuthorizationEngine();
        }

        protected IList<PolicyProperties> CreateResourcePolicies(int size)
        {
            var result = new List<PolicyProperties>();
            for (int i = 0; i < size; i++)
            {
                string id = $"policy_{i}";
                result.Add(new PolicyProperties(
                    new Policy(id) { Conditions = new List<PolicyCondition>() },
                    SystemIdGenerator.GenerateId(id)));
            }

            return result;
        }

        [TestClass]
        public class AnalyzeUserPreAuthorizationTests : PreAuthorizationEngineTests
        {
            [TestMethod]
            public async Task ShouldReturnZeroApprovalsWhenThereAreNoSharedPolicies()
            {
                DateTime userLastUpdated = DateTime.UtcNow;
                var userApprovedPolicies = new Dictionary<string, DateTime>();

                PreAuthorizationEngineResponse response = await PreAuthorizationEngine.AnalyzeUserPreAuthorization(
                    ResourcePolicies, userLastUpdated, userApprovedPolicies);

                Assert.IsTrue(response.UnExpiredPolicyApprovals.Count() == 0);
            }

            [TestMethod]
            public async Task ShouldReturnExpiredApprovalsDueToUserUpdate()
            {
                DateTime userLastUpdated = DateTime.UtcNow;
                var userApprovedPolicies = new Dictionary<string, DateTime>()
                {
                    { ResourcePolicies[0].Properties.Id, userLastUpdated.AddDays(-1) },
                    { ResourcePolicies[1].Properties.Id, userLastUpdated.AddDays(-1) }
                };

                PreAuthorizationEngineResponse response = await PreAuthorizationEngine.AnalyzeUserPreAuthorization(
                    ResourcePolicies, userLastUpdated, userApprovedPolicies);

                Assert.IsTrue(response.UnExpiredPolicyApprovals.Count() == 0);
                Assert.IsTrue(response.ExpiredPolicyApprovals.Count() == 2);
            }


            [TestMethod]
            public async Task ShouldReturnExpiredApprovalsDueToPolicyUpdate()
            {
                DateTime userLastUpdated = DateTime.UtcNow.AddDays(-2);
                var userApprovedPolicies = new Dictionary<string, DateTime>()
                {
                    { ResourcePolicies[0].Properties.Id,  DateTime.UtcNow.AddDays(-1) },
                    { ResourcePolicies[1].Properties.Id,  DateTime.UtcNow.AddDays(-1) }
                };

                ResourcePolicies[0].UpdateLastUpdated();
                ResourcePolicies[1].UpdateLastUpdated();

                PreAuthorizationEngineResponse response = await PreAuthorizationEngine.AnalyzeUserPreAuthorization(
                    ResourcePolicies, userLastUpdated, userApprovedPolicies);

                Assert.IsTrue(response.UnExpiredPolicyApprovals.Count() == 0);
                Assert.IsTrue(response.ExpiredPolicyApprovals.Count() == 2);
            }

            [TestMethod]
            public async Task ShouldReturnApprovalsWhenNotExpired()
            {
                DateTime userLastUpdated = DateTime.UtcNow.AddDays(-2);
                var userApprovedPolicies = new Dictionary<string, DateTime>()
                {
                    { ResourcePolicies[0].Properties.Id,  DateTime.UtcNow },
                    { ResourcePolicies[1].Properties.Id,  DateTime.UtcNow }
                };

                PreAuthorizationEngineResponse response = await PreAuthorizationEngine.AnalyzeUserPreAuthorization(
                    ResourcePolicies, userLastUpdated, userApprovedPolicies);

                Assert.IsTrue(response.UnExpiredPolicyApprovals.Count() == 2);
                Assert.IsTrue(response.ExpiredPolicyApprovals.Count() == 0);
            }
        }
    }
}
