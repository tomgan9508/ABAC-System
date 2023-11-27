using AuthorizationEngine.Engines;
using Common.Application;
using Common.Application.Models.API;
using Common.Cosmos.Models.Properties;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Service.Engines;

namespace Tests.AuthorizationEngineUnitTests
{
    [TestClass]
    public class AuthorizationEngineTests
    {
        protected Mock<IConditionChecker> ConditionCheckerMock;
        protected Mock<ILogger<PoliciesCheckerEngine>> LoggerMock;
        protected IPoliciesCheckerEngine PoliciesCheckerEngine;

        [TestInitialize]
        public void TestInitialize()
        {
            ConditionCheckerMock = new Mock<IConditionChecker>();
            LoggerMock = new Mock<ILogger<PoliciesCheckerEngine>>();

            PoliciesCheckerEngine = new PoliciesCheckerEngine(LoggerMock.Object, ConditionCheckerMock.Object);
        }

        protected IList<PolicyProperties> CreateResourcePolicies(int size)
        {
            var result = new List<PolicyProperties>();
            for (int i = 0; i < size; i++)
            {
                string id = $"policy_{i}";
                result.Add(new PolicyProperties(
                    new Policy(id) 
                    { 
                        Conditions = new List<PolicyCondition>()
                        {
                             new PolicyCondition($"attribute{i}", $"op{i}", $"value{i}")
                        }
                    },
                    SystemIdGenerator.GenerateId(id)));
            }

            return result;
        }

        [TestClass]
        public class PoliciesCheckerEngineTests : AuthorizationEngineTests
        {
            [TestMethod]
            public async Task ShouldAuthorizeWhenPolicyApproved()
            {                
                var policies = CreateResourcePolicies(5);
                var userAttributes = new Dictionary<string, object>();
                var userAttributeTypes = new Dictionary<string, string>();
                foreach (var policy in policies)
                {
                    userAttributes.Add(policy.Properties.Conditions.First().AttributeName, policy.Properties.Conditions.First().Value);
                    userAttributeTypes.Add(policy.Properties.Conditions.First().AttributeName, "string");
                }

                var expectedApprovedPolicy = policies[0];
                ConditionCheckerMock.Setup(x => x.Check(
                    It.IsAny<string>(), 
                    It.IsAny<object>(), 
                    It.Is<object>(value => value.ToString().Equals(expectedApprovedPolicy.Properties.Conditions[0].Value)),
                    It.IsAny<string>()))
                    .Returns(true);

                var response = await PoliciesCheckerEngine.Execute(policies, userAttributes, userAttributeTypes);

                Assert.IsTrue(response.ContainsFulfilledPolicy);
                Assert.IsTrue(response.PolicyFulfilledId == expectedApprovedPolicy.Properties.Id);
            }

            [TestMethod]
            public async Task ShouldNotAuthorizeWhenNoPolicyIsApproved()
            {
                var policies = CreateResourcePolicies(5);
                var userAttributes = new Dictionary<string, object>();
                var userAttributeTypes = new Dictionary<string, string>();
                foreach (var policy in policies)
                {
                    userAttributes.Add(policy.Properties.Conditions.First().AttributeName, policy.Properties.Conditions.First().Value);
                    userAttributeTypes.Add(policy.Properties.Conditions.First().AttributeName, "string");
                }

                var expectedApprovedPolicy = policies[0];
                ConditionCheckerMock.Setup(x => x.Check(
                    It.IsAny<string>(),
                    It.IsAny<object>(),
                    It.IsAny<object>(),
                    It.IsAny<string>()))
                    .Returns(false);

                var response = await PoliciesCheckerEngine.Execute(policies, userAttributes, userAttributeTypes);

                Assert.IsFalse(response.ContainsFulfilledPolicy);
                Assert.IsTrue(response.PolicyFulfilledId == string.Empty);
            }

            [TestMethod]
            public async Task ShouldThrowExceptionWhenConditionCheckerThrowsExcpetion()
            {
                var policies = CreateResourcePolicies(5);
                var userAttributes = new Dictionary<string, object>();
                var userAttributeTypes = new Dictionary<string, string>();
                foreach (var policy in policies)
                {
                    userAttributes.Add(policy.Properties.Conditions.First().AttributeName, policy.Properties.Conditions.First().Value);
                    userAttributeTypes.Add(policy.Properties.Conditions.First().AttributeName, "string");
                }

                var expectedApprovedPolicy = policies[0];
                ConditionCheckerMock.Setup(x => x.Check(
                    It.IsAny<string>(),
                    It.IsAny<object>(),
                    It.IsAny<object>(),
                    It.IsAny<string>()))
                    .Throws(new ArgumentException());

                await Assert.ThrowsExceptionAsync<ArgumentException>(() => PoliciesCheckerEngine.Execute(policies, userAttributes, userAttributeTypes));
            }

            [TestMethod]
            public async Task ShouldCancelAnalysisWhenAnotherPolicyAlreadyApproved()
            {
                const int maxConditionSize = 7; // use 7 conditions in total - 6 for one policy and 1 for another

                var policies = CreateResourcePolicies(2);
                var userAttributes = new Dictionary<string, object>();
                var userAttributeTypes = new Dictionary<string, string>();

                for (int i = 0; i < maxConditionSize; i++)
                {
                    string attributeName = $"attribute{i}";
                    userAttributes.Add(attributeName, $"value{i}");
                    userAttributeTypes.Add(attributeName, "string");

                    // already populdated the first 2 conditions
                    if (i >= 2)
                    {
                        policies[1].Properties.Conditions.Add(new PolicyCondition($"attribute{i}", $"op{i}", $"value{i}"));
                    }
                }

                var expectedApprovedPolicy = policies[0];
                ConditionCheckerMock.Setup(x => x.Check(
                    It.IsAny<string>(),
                    It.IsAny<object>(),
                    It.IsAny<object>(),
                    It.IsAny<string>()))
                    .Returns(true);

                var response = await PoliciesCheckerEngine.Execute(policies, userAttributes, userAttributeTypes);

                Assert.IsTrue(response.ContainsFulfilledPolicy);
                Assert.IsTrue(response.PolicyFulfilledId == expectedApprovedPolicy.Properties.Id);
                ConditionCheckerMock.Verify(x => x.Check(
                    It.IsAny<string>(),
                    It.IsAny<object>(),
                    It.IsAny<object>(),
                    It.IsAny<string>()), Times.AtMost(maxConditionSize - 1));
            }
        }

        [TestClass]
        public class ConditionCheckerTests : AuthorizationEngineTests
        {
            private IConditionChecker _conditionCheckerMock;

            [TestInitialize]
            public void TestInitialize()
            {
                _conditionCheckerMock = new ConditionChecker();
            }

            [TestMethod]
            public async Task ShouldReturnTrueForIntComparison()
            {
                string op = ">";
                object userAttribureValue = 5;
                object policyAttributeValue = 4;
                string type = "integer";

                Assert.IsTrue(_conditionCheckerMock.Check(op, userAttribureValue, policyAttributeValue, type));
            }

            [TestMethod]
            public async Task ShouldReturnTrueForStringComparison()
            {
                string op = "startsWith";
                object userAttribureValue = "superAdmin";
                object policyAttributeValue = "super";
                string type = "string";

                Assert.IsTrue(_conditionCheckerMock.Check(op, userAttribureValue, policyAttributeValue, type));
            }

            [TestMethod]
            public async Task ShouldReturnTrueForBoolComparison()
            {
                string op = "=";
                object userAttribureValue = true;
                object policyAttributeValue = true;
                string type = "boolean";

                Assert.IsTrue(_conditionCheckerMock.Check(op, userAttribureValue, policyAttributeValue, type));
            }

            [TestMethod]
            public async Task ShouldThrowExceptionWhenInvalidArguemnt()
            {
                string op = "<";
                object userAttribureValue = "superAdmin";
                object policyAttributeValue = "super";
                string type = "string";

                Assert.ThrowsException<ArgumentException>(() => 
                    _conditionCheckerMock.Check(op, userAttribureValue, policyAttributeValue, type));


                type = "object";
                Assert.ThrowsException<ArgumentException>(() => 
                    _conditionCheckerMock.Check(op, userAttribureValue, policyAttributeValue, type));
            }
        }
    }
}