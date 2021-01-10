using CTRL.Portal.API.APIConstants;
using CTRL.Portal.API.Configuration;
using CTRL.Portal.API.Contracts;
using CTRL.Portal.API.EntityContexts;
using CTRL.Portal.API.Exceptions;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace CTRL.Portal.API.Services
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly AuthenticationConfiguration _authenticationConfiguration;

        public AuthenticationService(
            UserManager<ApplicationUser> userManager,
            AuthenticationConfiguration authenticationConfiguration)
        {
            _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
            _authenticationConfiguration = authenticationConfiguration ?? throw new ArgumentNullException(nameof(authenticationConfiguration));
        }

        public async Task<AuthenticationResponseContract> Login(LoginContract loginContract)
        {
            ValidateOnLogin(loginContract);

            var user = await _userManager.FindByNameAsync(loginContract.UserName);

            if (user != null && await _userManager.CheckPasswordAsync(user, loginContract.Password))
            {
                var userRoles = await _userManager.GetRolesAsync(user);
                var authClaims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, user.UserName),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
                };

                authClaims.AddRange(userRoles.Select(role => new Claim(ClaimTypes.Role, role)));

                return new AuthenticationResponseContract
                {
                    Message = ApiMessages.LoginSuccessful,
                    Status = HttpStatusCode.OK,
                    Token = new JwtSecurityTokenHandler().WriteToken(GenerateToken(authClaims)),
                    UserName = loginContract.UserName
                };
            }

            throw new InvalidLoginAttemptException(ApiMessages.InvalidCredentials);
        }

        public async Task Register(RegistrationContract registrationContract)
        {
            ValidateOnRegister(registrationContract);

            if((await _userManager.FindByNameAsync(registrationContract.UserName) != null))
                throw new InvalidOperationException(ApiMessages.UserAlreadyExists);

            var user = new ApplicationUser
            {
                Email = registrationContract.Email,
                UserName = registrationContract.UserName,
                SecurityStamp = Guid.NewGuid().ToString()
            };

            var result = await _userManager.CreateAsync(user, registrationContract.Password);

            if (!result?.Succeeded ?? false)
            {
                if (result?.Errors?.Any() ?? false)
                {
                    throw new InvalidOperationException(string.Join(",", result.Errors.Select(e => e.Description)));
                }

                throw new InvalidOperationException(ApiMessages.UnhandledErrorCreatingUser);
            }
        }

        private JwtSecurityToken GenerateToken(List<Claim> authClaims)
        {
            var authSignInKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_authenticationConfiguration.Secret));
            return new JwtSecurityToken(
                _authenticationConfiguration.ValidIssuer,
                _authenticationConfiguration.ValidAudience,
                authClaims,
                expires: DateTime.Now.Add(TimeSpan.Parse(_authenticationConfiguration.Expires)),
                signingCredentials: new SigningCredentials(authSignInKey, SecurityAlgorithms.HmacSha256));
        }

        private static void ValidateOnLogin(LoginContract loginContract)
        {
            if (loginContract is null 
                ||string.IsNullOrWhiteSpace(loginContract.UserName)
                ||string.IsNullOrWhiteSpace(loginContract.Password)) throw new ArgumentException(nameof(loginContract));
        }

        private static void ValidateOnRegister(RegistrationContract registrationContract)
        {
            if (registrationContract is null 
                ||string.IsNullOrWhiteSpace(registrationContract.UserName) 
                ||string.IsNullOrWhiteSpace(registrationContract.Email) 
                ||string.IsNullOrWhiteSpace(registrationContract.Password)) throw new ArgumentException(nameof(registrationContract));
        }
    }
}
