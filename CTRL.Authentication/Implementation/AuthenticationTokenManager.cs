using CTRL.Authentication.Configuration;
using CTRL.Authentication.Contracts;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Principal;
using System.Text;

namespace CTRL.Authentication.Implementation
{
    public class AuthenticationTokenManager : IAuthenticationTokenManager
    {
        private readonly AuthenticationParameters _authenticationParameters;
        private readonly AuthenticationConfiguration _authenticationConfiguration;

        public AuthenticationTokenManager(AuthenticationParameters authenticationParameters, AuthenticationConfiguration authenticationConfiguration)
        {
            _authenticationParameters = authenticationParameters ?? throw new ArgumentNullException(nameof(authenticationParameters));
            _authenticationConfiguration = authenticationConfiguration ?? throw new ArgumentNullException(nameof(authenticationConfiguration));
        }

        public JwtSecurityToken GenerateToken(List<Claim> authClaims)
        {
            var authSignInKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_authenticationConfiguration.Secret));

            return new JwtSecurityToken(
                _authenticationConfiguration.ValidIssuer,
                _authenticationConfiguration.ValidAudience,
                authClaims,
                expires: DateTime.Now.Add(TimeSpan.Parse(_authenticationConfiguration.Expires)),
                signingCredentials: new SigningCredentials(authSignInKey, SecurityAlgorithms.HmacSha256));
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
