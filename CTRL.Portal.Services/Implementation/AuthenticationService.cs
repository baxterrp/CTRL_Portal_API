using CTRL.Portal.API.EntityContexts;
using CTRL.Portal.Common.Constants;
using CTRL.Portal.Common.Contracts;
using CTRL.Portal.Common.Exceptions;
using CTRL.Portal.Data.DTO;
using CTRL.Portal.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Threading.Tasks;

namespace CTRL.Portal.Services.Implementation
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IAuthenticationTokenManager _authenticationTokenManager;
        private readonly IAccountService _accountService;
        private readonly IUserSettingsService _userSettingsService;

        public AuthenticationService(
            UserManager<ApplicationUser> userManager, 
            IAuthenticationTokenManager authenticationTokenManager, 
            IAccountService accountService, 
            IUserSettingsService userSettingsService)
        {
            _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
            _authenticationTokenManager = authenticationTokenManager ?? throw new ArgumentNullException(nameof(authenticationTokenManager));
            _accountService = accountService ?? throw new ArgumentNullException(nameof(accountService));
            _userSettingsService = userSettingsService ?? throw new ArgumentNullException(nameof(userSettingsService));
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

                var accountResponse = _accountService.GetAccounts(user.UserName);
                var userSettingsResponse = _userSettingsService.GetUserSettings(user.UserName);

                List<Task> tasks = new List<Task>
                {
                    accountResponse,
                    userSettingsResponse
                };

                await Task.WhenAll(tasks);

                var userSettings = (userSettingsResponse?.IsCompletedSuccessfully ?? false) ? new UserSettings
                {
                    UserName = userSettingsResponse.Result.UserName,
                    Id = userSettingsResponse.Result.Id,
                    DefaultAccount = userSettingsResponse.Result.DefaultAccount,
                    Theme = userSettingsResponse.Result.Theme
                } : null;

                return new AuthenticationResponseContract
                {
                    Message = ApiMessages.LoginSuccessful,
                    Status = HttpStatusCode.OK,
                    Token = new JwtSecurityTokenHandler().WriteToken(_authenticationTokenManager.GenerateToken(authClaims)),
                    UserName = loginContract.UserName,
                    UserSettings = userSettings,
                    Accounts = (accountResponse?.IsCompletedSuccessfully ?? false) ? accountResponse.Result.Select(a => new Account
                    {
                        Id = a.Id,
                        Name = a.Name
                    }) : new List<Account>()
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

            var createUserResult = _userManager.CreateAsync(user, registrationContract.Password);
            var saveSettingsResult = _userSettingsService.SaveSettings(new UserSettingsDto
            {
                UserName = user.UserName,
                Theme = null
            });

            List<Task> tasks = new List<Task>
            {
                createUserResult,
                saveSettingsResult
            };

            await Task.WhenAll(tasks);

            if (!createUserResult?.Result?.Succeeded ?? true)
            {
                if (createUserResult?.Result?.Errors?.Any() ?? true)
                {
                    throw new InvalidOperationException(string.Join(",", 
                        createUserResult?.Result?.Errors?.Select(e => e.Description) ?? new List<string> { ApiMessages.UnhandledErrorCreatingUser }));
                }

                throw new InvalidOperationException(ApiMessages.UnhandledErrorCreatingUser);
            }
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
