using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Principal;

namespace CTRL.Portal.API.Services
{
    public interface IAuthenticationTokenManager
    {
        IPrincipal ValidateToken(string token);
        JwtSecurityToken GenerateToken(List<Claim> authClaims);
    }
}