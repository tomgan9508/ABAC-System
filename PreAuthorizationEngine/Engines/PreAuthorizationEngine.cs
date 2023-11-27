using Common.Application.Dtos.Authorization;
using Common.Cosmos.Models.Properties;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PreAuthorization.Engines
{
    public class PreAuthorizationEngine : IPreAuthorizationEngine
    {
        public Task<PreAuthorizationEngineResponse> AnalyzeUserPreAuthorization(
            IList<PolicyProperties> resourcePolicies,
            DateTime userLastUpdated,
            IDictionary<string, DateTime> userApprovedPolicies)
        {
            PreAuthorizationEngineResponse result = new();

            if (!ArePoliciesShared(userApprovedPolicies, resourcePolicies, out IDictionary<string, DateTime> sharedApprovedPolicies))
            {
                return Task.FromResult(result);
            }

            CheckPolicyApprovalsExpiration(sharedApprovedPolicies,
                                           userLastUpdated,
                                           resourcePolicies.ToDictionary(policy => policy.Properties.Id),
                                           out IList<string> expiredPolicyApprovals,
                                           out IList<string> unExpiredPolicyApprovals);

            result.UnExpiredPolicyApprovals = unExpiredPolicyApprovals;
            result.ExpiredPolicyApprovals = expiredPolicyApprovals;

            return Task.FromResult(result);
        }

        // This method checks if there are any pre-approved or already expired policies for the resource.
        // Expired policies are policies that were approved before the last update of the user or of the policy.
        private void CheckPolicyApprovalsExpiration(
            IDictionary<string, DateTime> userApprovedPolicies,
            DateTime userLastUpdated,
            IDictionary<string, PolicyProperties> resourcePolicies, 
            out IList<string> expiredPolicyApprovals,
            out IList<string> unExpiredPolicyApprovals)
        {
            expiredPolicyApprovals = new List<string>();
            unExpiredPolicyApprovals = new List<string>();

            foreach (KeyValuePair<string, DateTime> approvedPolicy in userApprovedPolicies)
            {
                if (resourcePolicies.ContainsKey(approvedPolicy.Key))
                {
                    if (approvedPolicy.Value > userLastUpdated &&
                        approvedPolicy.Value > resourcePolicies[approvedPolicy.Key].LastUpdated)
                    {
                        unExpiredPolicyApprovals.Add(approvedPolicy.Key);
                    }
                    else
                    {
                        expiredPolicyApprovals.Add(approvedPolicy.Key);
                    }
                }
            }
        }

        private bool ArePoliciesShared(
            IDictionary<string, DateTime> userApprovedPolicies,
            IList<PolicyProperties> resourcePolicies,
            out IDictionary<string, DateTime> sharedPolicies)
        {
            var sharedPolicyIds = userApprovedPolicies.Keys.Intersect(resourcePolicies.Select(policy => policy.Properties.Id));
            sharedPolicies = userApprovedPolicies.Where(kvp => sharedPolicyIds.Any(id => id == kvp.Key)).ToDictionary(kvp => kvp.Key, kvp => kvp.Value);

            return sharedPolicies.Any();
        }

    }
}
