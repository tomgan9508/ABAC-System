using Common.Application.Dtos.Authorization;
using Common.Cosmos.Models.Properties;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AuthorizationEngine.Engines
{
    /// <summary>
    /// PoliciesCheckerEngine is responsible for checking if 
    /// certain attributes meet the requirements of a policy.
    /// </summary>
    public interface IPoliciesCheckerEngine
    {
        /// <summary>
        /// This method is reposible for checking if certain user attributes
        /// meet the requirements of a policy.
        /// <param name="policies">The resource's policies</param>
        /// <param name="userAttributes">The user's attributes</param>
        /// <param name="userAttributeTypes">The user's attribute types</param>
        /// <returns>The <!---->AuthorizationEngineResponse> response</returns>
        /// </summary>
        Task<AuthorizationEngineResponse> Execute(
            IList<PolicyProperties> policies,
            IDictionary<string, object> userAttributes,
            IDictionary<string, string> userAttributeTypes);
    }
}
