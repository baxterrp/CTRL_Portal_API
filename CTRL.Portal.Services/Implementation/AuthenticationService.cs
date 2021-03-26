using CTRL.Authentication;
using CTRL.Authentication.Exceptions;
using CTRL.Portal.API.EntityContexts;
using CTRL.Portal.Common.Constants;
using CTRL.Portal.Common.Contracts;
using CTRL.Portal.Data.DTO;
using CTRL.Portal.Services.Constants;
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
        private readonly string _senderDomain;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ICodeService _codeService;
        private readonly IEmailProvider _emailProvider;
        private readonly IAuthenticationTokenManager _authenticationTokenManager;
        private readonly IBusinessEntityService _accountService;
        private readonly IUserSettingsService _userSettingsService;

        public AuthenticationService(
            string senderDomain,
            UserManager<ApplicationUser> userManager,
            ICodeService codeService,
            IEmailProvider emailProvider,
            IAuthenticationTokenManager authenticationTokenManager,
            IBusinessEntityService accountService,
            IUserSettingsService userSettingsService)
        {
            _senderDomain = !string.IsNullOrWhiteSpace(senderDomain) ? senderDomain : throw new ArgumentNullException(nameof(senderDomain));
            _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
            _codeService = codeService ?? throw new ArgumentNullException(nameof(codeService));
            _emailProvider = emailProvider ?? throw new ArgumentNullException(nameof(emailProvider));
            _authenticationTokenManager = authenticationTokenManager ?? throw new ArgumentNullException(nameof(authenticationTokenManager));
            _accountService = accountService ?? throw new ArgumentNullException(nameof(accountService));
            _userSettingsService = userSettingsService ?? throw new ArgumentNullException(nameof(userSettingsService));
        }

        public async Task ActivateUserAccount(UserAccountActivationContract userAccountActivationContract)
        {
            ValidateOnActivate(userAccountActivationContract);

            var user = await _userManager.FindByNameAsync(userAccountActivationContract.UserName);

            if (user is null)
            {
                throw new ResourceNotFoundException($"No user found with name {userAccountActivationContract.UserName}");
            }

            var codeIsValid = await _codeService.ValidateCode(user.Email, userAccountActivationContract.Code);

            if (!codeIsValid)
            {
                throw new InvalidOperationException($"{userAccountActivationContract.Code} is not a valid code for {user.UserName}");
            }

            var settings = await _userSettingsService.GetUserSettings(user.UserName);

            settings.IsActive = true;

            await _userSettingsService.SaveSettings(settings);
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

                var accountResponse = _accountService.GetBusinessEntities(user.UserName);
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
                    DefaultAccount = userSettingsResponse.Result.DefaultBusinessEntity,
                    Theme = userSettingsResponse.Result.Theme,
                    IsActive = userSettingsResponse.Result.IsActive
                } : null;

                if (!userSettings.IsActive)
                {
                    throw new InvalidLoginAttemptException("Must activate account to login");
                }

                return new AuthenticationResponseContract
                {
                    Message = ApiMessages.LoginSuccessful,
                    Status = HttpStatusCode.OK,
                    Token = new JwtSecurityTokenHandler().WriteToken(_authenticationTokenManager.GenerateToken(authClaims)),
                    UserName = loginContract.UserName,
                    UserSettings = userSettings,
                    Businesses = (accountResponse?.IsCompletedSuccessfully ?? false) ? accountResponse.Result.Select(a => new BusinessEntity
                    {
                        Id = a.Id,
                        Name = a.Name
                    }) : new List<BusinessEntity>()
                };
            }

            throw new InvalidLoginAttemptException(ApiMessages.InvalidCredentials);
        }

        public async Task Register(RegistrationContract registrationContract)
        {
            ValidateOnRegister(registrationContract);

            if (await _userManager.FindByNameAsync(registrationContract.UserName) != null)
            {
                throw new InvalidOperationException(string.Format(ApiMessages.UserAlreadyExists, "UserName"));
            }

            if (await _userManager.FindByEmailAsync(registrationContract.Email) != null)
            {
                throw new InvalidOperationException(string.Format(ApiMessages.UserAlreadyExists, "Email"));
            }

            var user = new ApplicationUser
            {
                Email = registrationContract.Email,
                UserName = registrationContract.UserName,
                SecurityStamp = Guid.NewGuid().ToString()
            };

            var verficationCodeResult = _codeService.SaveCode(registrationContract.Email);
            var createUserResult = _userManager.CreateAsync(user, registrationContract.Password);
            var saveSettingsResult = _userSettingsService.SaveSettings(new UserSettingsDto
            {
                UserName = user.UserName,
                Theme = null,
                IsActive = false
            });

            List<Task> tasks = new List<Task>
            {
                createUserResult,
                saveSettingsResult,
                verficationCodeResult
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

            await _emailProvider.SendEmail(new VerifyRegistrationEmail
            {
                Header = "Verify your account registration",
                Name = registrationContract.UserName,
                Recipient = registrationContract.Email,
                ViewName = EmailTemplateNames.RegistrationVerification,
                VerificationLink =
                    $"{_senderDomain}{string.Format(GeneralConstants.VerifiyAccountRegistration, registrationContract.UserName, verficationCodeResult.Result.Code)}"
            });
        }

        private static void ValidateOnLogin(LoginContract loginContract)
        {
            if (loginContract is null
                || string.IsNullOrWhiteSpace(loginContract.UserName)
                || string.IsNullOrWhiteSpace(loginContract.Password))
            {
                throw new ArgumentException(nameof(loginContract));
            }
        }

        private static void ValidateOnRegister(RegistrationContract registrationContract)
        {
            if (registrationContract is null
                || string.IsNullOrWhiteSpace(registrationContract.UserName)
                || string.IsNullOrWhiteSpace(registrationContract.Email)
                || string.IsNullOrWhiteSpace(registrationContract.Password))
            {
                throw new ArgumentException(nameof(registrationContract));
            }
        }

        private void ValidateOnActivate(UserAccountActivationContract userAccountActivationContract)
        {
            if (userAccountActivationContract is null)
            {
                throw new ArgumentNullException(nameof(userAccountActivationContract));
            }

            if (string.IsNullOrWhiteSpace(userAccountActivationContract.UserName))
            {
                throw new ArgumentException("Username cannot be null or empty", nameof(userAccountActivationContract.UserName));
            }

            if (string.IsNullOrWhiteSpace(userAccountActivationContract.Code))
            {
                throw new ArgumentException("Code cannot be null or empty", nameof(userAccountActivationContract.Code));
            }
        }
    }
}
