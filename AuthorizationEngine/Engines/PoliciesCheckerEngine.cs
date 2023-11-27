using Common.Application.Dtos.Authorization;
using Common.Cosmos.Models.Properties;
using Microsoft.Extensions.Logging;
using Service.Engines;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace AuthorizationEngine.Engines
{
    public class PoliciesCheckerEngine : IPoliciesCheckerEngine
    {
        private readonly ILogger<PoliciesCheckerEngine> _logger;
        private readonly IConditionChecker _conditionChecker;

        public PoliciesCheckerEngine(ILogger<PoliciesCheckerEngine> logger, IConditionChecker conditionChecker)
        {
            _logger = logger;
            _conditionChecker = conditionChecker;
        }

        public async Task<AuthorizationEngineResponse> Execute(
            IList<PolicyProperties> policies, 
            IDictionary<string, object> userAttributes,
            IDictionary<string, string> userAttributeTypes)
        {
            AuthorizationEngineResponse response = new();
            var cts = new CancellationTokenSource();
            ParallelOptions options = new() { MaxDegreeOfParallelism = Environment.ProcessorCount, CancellationToken = cts.Token };

            try
            {
                // run the policies analysis in parallel
                await Parallel.ForEachAsync(policies, options, async (policy, ct) =>
                {
                    if (await IsPolicyFulfilled(policy, userAttributes, userAttributeTypes, cts))
                    {
                        response.ContainsFulfilledPolicy = true;
                        response.PolicyFulfilledId = policy.Properties.Id;
                        if (cts.Token.CanBeCanceled)
                        {
                            cts.Cancel();
                        }
                    }
                });
            }
            catch (TaskCanceledException)
            {
                _logger.LogInformation("Operation was canceled since a fulfilled policy was found.");
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, $"Error occured during execution. Error: {ex.Message}");
                throw;
            }

            return response;
        }

        private ValueTask<bool> IsPolicyFulfilled(
            PolicyProperties policy, 
            IDictionary<string, object> attributes,
            IDictionary<string, string> attributeTypes,
            CancellationTokenSource cts)
        {
            foreach (PolicyCondition condition in policy.Properties.Conditions)
            {
                if (cts.IsCancellationRequested)
                {
                    return new ValueTask<bool>(false);
                }

                if (!IsConditionFulfilled(condition, attributes, attributeTypes))
                {
                    return new ValueTask<bool>(false);
                }
            }

            return new ValueTask<bool>(true);
        }

        private bool IsConditionFulfilled(PolicyCondition condition, IDictionary<string, object> attributes, IDictionary<string, string> attributeTypes)
        {
            if (condition is null || condition.Operator is null ||
                condition.Value is null || condition.AttributeName is null)
            {
                return false;
            }

            if (!attributes.ContainsKey(condition.AttributeName) ||
                !attributeTypes.ContainsKey(condition.AttributeName))
            {
                return false;
            }
            
            return _conditionChecker.Check(
                condition.Operator, 
                attributes[condition.AttributeName], 
                condition.Value, 
                attributeTypes[condition.AttributeName]);
        }
    }
}
