using Common.Application.Models.API;
using System.Threading.Tasks;

namespace AuthorizationManagement.ResourceManager
{
    /// <summary>
    /// AuthorizationManager is responsible for managing the authorizations in the system.
    /// </summary>
    public interface IAuthorizationManager
    {
        /// <summary>
        /// This method is responsoble for checking if a user is 
        /// authorized to access a specified resource.
        /// </summary>
        Task<AuthorizationResponse> IsAuthorizedAsync(string userId, string resourceId);
    }
}
