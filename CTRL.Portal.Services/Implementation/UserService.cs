using CTRL.Portal.API.EntityContexts;
using CTRL.Portal.Common.Constants;
using CTRL.Portal.Common.Contracts;
using CTRL.Portal.Common.Exceptions;
using CTRL.Portal.Data.DTO;
using CTRL.Portal.Services.Constants;
using CTRL.Portal.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using System;
using System.Threading.Tasks;

namespace CTRL.Portal.Services.Implementation
{
    public class UserService : IUserService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ICodeService _codeService;
        private readonly IEmailProvider _emailProvider;

        public UserService(UserManager<ApplicationUser> userManager, ICodeService codeService, IEmailProvider emailProvider)
        {
            _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
            _codeService = codeService ?? throw new ArgumentNullException(nameof(codeService));
            _emailProvider = emailProvider ?? throw new ArgumentNullException(nameof(emailProvider));
        }

        public async Task DeleteUser(string userName)
        {
            if (string.IsNullOrWhiteSpace(userName))
            {
                throw new ArgumentNullException(nameof(userName));
            }

            var user = await _userManager.FindByNameAsync(userName);

            if (user is null)
            {
                throw new ResourceNotFoundException($"No user found with name {userName}");
            }

            var result = await _userManager.DeleteAsync(user);

            if (!result.Succeeded)
            {
                throw new InvalidOperationException($"Could not delete user {userName}");
            }
        }

        public async Task RequestPasswordReset(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                throw new ArgumentNullException(nameof(email));
            }

            var code = await _codeService.SaveCode(email);

            await _emailProvider.SendEmail(GetCodeEmail(email, code));
        }

        public async Task ResetPassword(ResetPasswordContract resetPasswordContract)
        {
            ValidateResetPasswordContract(resetPasswordContract);

            var user = await _userManager.FindByNameAsync(resetPasswordContract.UserName);

            if (user is null)
            {
                throw new InvalidOperationException(ApiMessages.InvalidCredentials);
            }

            var codeIsValid = await _codeService.ValidateCode(user.Email, resetPasswordContract.Code);

            if (!codeIsValid)
            {
                throw new InvalidOperationException(ApiMessages.InvalidCredentials);
            }

            var resetToken = await _userManager.GeneratePasswordResetTokenAsync(user);

            var result = await _userManager.ResetPasswordAsync(user, resetToken, resetPasswordContract.NewPassword);

            if (!result.Succeeded)
            {
                throw new InvalidOperationException(ApiMessages.InvalidCredentials);
            }
        }

        private static void ValidateResetPasswordContract(ResetPasswordContract resetPasswordContract)
        {
            if (resetPasswordContract is null) throw new ArgumentNullException(nameof(resetPasswordContract));
            if (string.IsNullOrWhiteSpace(resetPasswordContract.UserName)) throw new ArgumentException(nameof(resetPasswordContract.UserName));
            if (string.IsNullOrWhiteSpace(resetPasswordContract.NewPassword)) throw new ArgumentException(nameof(resetPasswordContract.NewPassword));
            if (string.IsNullOrWhiteSpace(resetPasswordContract.Code)) throw new ArgumentException(nameof(resetPasswordContract.Code));
        }

        private static ResetPasswordEmailContract GetCodeEmail(string email, PersistedCodeDto code) => 
            new ResetPasswordEmailContract
            {
                Header = $"Password Reset Requested for {email}",
                Name = email,
                Recipient = email,
                ViewName = EmailTemplateNames.ResetPassword,
                ResetCode = code.Code
            };
    }
}
