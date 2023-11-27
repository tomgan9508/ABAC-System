using Common.Application.Dtos.Authorization;
using Common.Cosmos.Models.Properties;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PreAuthorization.Engines
{
    /// <summary>
    /// PreAuthorizationEngine is responsible for analyzing the user's pre-authorization state.
    /// </summary>
    public interface IPreAuthorizationEngine
    {
        /// <summary>
        /// This method analyzes the user's pre-authorization state using the following inputs:
        /// <param name="resourcePolicies"> The list of policies that are associated with the resource.
        /// <param name="userLastUpdated"> The last time the user was updated.
        /// <param name="userApprovedPolicies"> The list of policies that the user has been approved of.
        /// <returns>The <!---->PreAuthorizationEngineResponse> response</returns>
        /// </summary>
        Task<PreAuthorizationEngineResponse> AnalyzeUserPreAuthorization(
                    IList<PolicyProperties> resourcePolicies,
                    DateTime userLastUpdated,
                    IDictionary<string, DateTime> userApprovedPolicies);
    }
}
