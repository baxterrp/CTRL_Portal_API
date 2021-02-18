using CTRL.Portal.API.APIConstants;
using CTRL.Portal.API.Contracts;
using CTRL.Portal.API.EntityContexts;
using CTRL.Portal.Data.DataExceptions;
using CTRL.Portal.Data.DTO;
using CTRL.Portal.Data.Repositories;
using Microsoft.AspNetCore.Identity;
using System;
using System.Threading.Tasks;

namespace CTRL.Portal.API.Services
{
    public class UserService : IUserService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ICodeRepository _codeRepository;
        private readonly UtilityManager _utilityManager;

        public UserService(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
        }

        public async Task DeleteUser(string userName)
        {
            if (string.IsNullOrWhiteSpace(userName)) throw new ArgumentNullException(nameof(userName));

            var user = await _userManager.FindByNameAsync(userName);

            if (user is null) throw new ResourceNotFoundException($"No user found with name {userName}");

            var result = await _userManager.DeleteAsync(user);

            if (!result.Succeeded)
                throw new InvalidOperationException($"Could not delete user {user}");
        }

        public async Task ResetPassword(ResetPasswordContract resetPasswordContract)
        {
            if (resetPasswordContract is null) throw new ArgumentNullException(nameof(resetPasswordContract));
            if (string.IsNullOrWhiteSpace(resetPasswordContract.UserName)) throw new ArgumentException(nameof(resetPasswordContract.UserName));
            if (string.IsNullOrWhiteSpace(resetPasswordContract.NewPassword)) throw new ArgumentException(nameof(resetPasswordContract.NewPassword));

            var user = await _userManager.FindByNameAsync(resetPasswordContract.UserName);
            var resetToken = await _userManager.GeneratePasswordResetTokenAsync(user);

            var result = await _userManager.ResetPasswordAsync(user, resetToken, resetPasswordContract.NewPassword);

            if (!result.Succeeded)
                throw new InvalidOperationException(ApiMessages.InvalidPassword);
        }
        
        //public async Task<PersistCode> GenerateCode(string email)
        //{
        //    var resetCode =  _utilityManager.GenerateCode(6);
        //    var expiration = DateTime.Now.AddMinutes(10);

        //    //var saveCodeResult = await _codeRepository.SaveCode(new PersistCode
        //    //{
        //    //});

        //    return new PersistCode
        //    {
        //        Email = email,
        //        Expiration = expiration,
        //        ResetCode = resetCode,

        //    };
        //}

        public async Task SaveCode(PersistCode persistCode)
        {
            await _codeRepository.SaveCode(persistCode);

        }

        public async Task<PersistCode> SavePersistCode(string email) //modeled after the account display DTO workflow
        {
            var resetCode =  _utilityManager.GenerateCode(6);
            var expiration = DateTime.Now.AddMinutes(10);

            await _codeRepository.SavePersistCode(email, new PersistCode
            {
                Email = email,
                Expiration = expiration,
                ResetCode = resetCode,

            });

            return new PersistCode
            {
                Email = email,
                Expiration = expiration,
                ResetCode = resetCode,

            };

        }

       

    }
}
