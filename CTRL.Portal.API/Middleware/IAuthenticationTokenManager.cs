using System.Security.Principal;

namespace CTRL.Portal.API.Middleware
{
    public interface IAuthenticationTokenManager
    {
        IPrincipal ValidateToken(string token);
    }
}