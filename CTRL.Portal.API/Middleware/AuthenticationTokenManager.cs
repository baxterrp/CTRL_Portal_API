using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Principal;
using System.Text;

namespace CTRL.Portal.API.Middleware
{
    public class AuthenticationTokenManager : IAuthenticationTokenManager
    {
        private readonly AuthenticationParameters _authenticationParameters;

        public AuthenticationTokenManager(AuthenticationParameters authenticationParameters)
        {
            _authenticationParameters = authenticationParameters ?? throw new ArgumentNullException(nameof(authenticationParameters));
        }

        public IPrincipal ValidateToken(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var validationParameters = GetValidationParameters();
            try
            {
                return tokenHandler.ValidateToken(token, validationParameters, out SecurityToken securityToken);
            }
            catch
            {
                return null;
            }
        }

        private TokenValidationParameters GetValidationParameters() => new TokenValidationParameters
        {
            ValidateLifetime = true,
            ValidateAudience = true,
            ValidateIssuer = true,
            RequireExpirationTime = true,
            ClockSkew = TimeSpan.Zero,
            ValidIssuer = _authenticationParameters.Issuer,
            ValidAudience = _authenticationParameters.Audience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_authenticationParameters.Key))
        };
    }
}
