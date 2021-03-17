using CTRL.Portal.API.Contracts;
using CTRL.Portal.API.EntityContexts;
using CTRL.Portal.API.Services;
using CTRL.Portal.Common.Constants;
using CTRL.Portal.Data.DataExceptions;
using CTRL.Portal.Data.DTO;
using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CTRL.Portal.API.UnitTests
{
    [TestClass]
    public class UserServiceUnitTests
    {
        private readonly Mock<UserManager<ApplicationUser>> _mockUserManager;
        private readonly Mock<ICodeService> _mockCodeService;
        private readonly Mock<IEmailProvider> _mockEmailProvider;
        private readonly UserService _sut;

        private static readonly string _testEmail = "test@test.com";
        private static readonly string _userName = "test_user";
        private static readonly string _testCode = "ABC123";
        private static readonly string _testPass = "test_password";
        private static readonly string _testGuid = Guid.NewGuid().ToString();

        public UserServiceUnitTests()
        {
            _mockUserManager = GetMockUserManager();
            _mockCodeService = new Mock<ICodeService>();
            _mockEmailProvider = new Mock<IEmailProvider>();

            _sut = new UserService(_mockUserManager.Object, _mockCodeService.Object, _mockEmailProvider.Object);
        }

        #region DeleteUser

        [TestMethod]
        public void DeleteUserThrowsIfUserNameEmpty()
        {
            var userName = string.Empty;

            Func<Task> func = async () => await _sut.DeleteUser(userName);

            func.Should().Throw<ArgumentNullException>().And.ParamName.Should().Be("userName");
        }

        [TestMethod]
        public void DeleteUserThrowsIfNoUserFound()
        {
            var message = $"No user found with name {_userName}";
            _mockUserManager.Setup(manager => manager.FindByNameAsync(_userName)).ReturnsAsync((ApplicationUser)null);

            Func<Task> func = async () => await _sut.DeleteUser(_userName);

            func.Should().Throw<ResourceNotFoundException>().WithMessage(message);
        }

        [TestMethod]
        public void DeleteUserThrowsIfDeleteFails()
        {
            var message = $"Could not delete user {_userName}";
            var failedResult = IdentityResult.Failed(null);

            _mockUserManager.Setup(manager => manager.FindByNameAsync(_userName)).ReturnsAsync(new ApplicationUser());
            _mockUserManager.Setup(manager => manager.DeleteAsync(It.IsAny<ApplicationUser>())).ReturnsAsync(failedResult);

            Func<Task> func = async () => await _sut.DeleteUser(_userName);

            func.Should().Throw<InvalidOperationException>().WithMessage(message);
        }

        [TestMethod]
        public async Task DeleteUserRunsSuccessfully()
        {
            var successResult = IdentityResult.Success;

            _mockUserManager.Setup(manager => manager.FindByNameAsync(_userName)).ReturnsAsync(new ApplicationUser());
            _mockUserManager.Setup(manager => manager.DeleteAsync(It.IsAny<ApplicationUser>())).ReturnsAsync(successResult);

            var task = _sut.DeleteUser(_userName);
            await task;

            _mockUserManager.Verify(manager => manager.FindByNameAsync(_userName), Times.Once);
            _mockUserManager.Verify(manager => manager.DeleteAsync(It.IsAny<ApplicationUser>()), Times.Once);

            task.IsCompletedSuccessfully.Should().BeTrue();
        }

        #endregion

        #region RequestPasswordReset

        [TestMethod]
        public void RequestResetPasswordThrowsIfEmailInvalid()
        {
            var email = string.Empty;

            Func<Task> func = async () => await _sut.RequestPasswordReset(email);

            func.Should().Throw<ArgumentNullException>().And.ParamName.Should().Be("email");
        }

        [TestMethod]
        public async Task RequestPasswordSendsCode()
        {
            _mockCodeService.Setup(s => s.SaveCode(_testEmail)).ReturnsAsync(GetResetCode());
            _mockEmailProvider.Setup(p => p.SendEmail(It.IsAny<EmailContract>()));

            var task = _sut.RequestPasswordReset(_testEmail);
            await task;

            _mockCodeService.Verify(s => s.SaveCode(_testEmail), Times.Once);
            _mockEmailProvider.Verify(p => p.SendEmail(It.IsAny<EmailContract>()), Times.Once);

            task.IsCompletedSuccessfully.Should().BeTrue();
        }

        #endregion

        #region ResetPassword

        [TestMethod]
        [DynamicData(nameof(GetInvalidResetPasswordContracts), DynamicDataSourceType.Method)]
        public void ResetPasswordThrowsWithInvalidPasswordContract(ResetPasswordContract resetPasswordContract)
        {
            Func<Task> func = async () => await _sut.ResetPassword(resetPasswordContract);

            switch (resetPasswordContract)
            {
                case var contract when contract is null:
                    func.Should().Throw<ArgumentNullException>().And.ParamName.Should().Be("resetPasswordContract");
                    break;

                case var contract when string.IsNullOrWhiteSpace(contract.NewPassword):
                    func.Should().Throw<ArgumentException>().And.Message.Should().Contain("NewPassword");
                    break;

                case var contract when string.IsNullOrWhiteSpace(contract.UserName):
                    func.Should().Throw<ArgumentException>().And.Message.Should().Contain("UserName");
                    break;

                case var contract when string.IsNullOrWhiteSpace(contract.Code):
                    func.Should().Throw<ArgumentException>().And.Message.Should().Contain("Code");
                    break;
            }
        }

        [TestMethod]
        public void ResetPasswordThrowsIfNoUserFound()
        {
            _mockUserManager.Setup(m => m.FindByNameAsync(It.IsAny<string>())).ReturnsAsync((ApplicationUser)null);

            Func<Task> func = async () => await _sut.ResetPassword(GetResetPasswordContract());

            func.Should().Throw<InvalidOperationException>().WithMessage(ApiMessages.InvalidCredentials);
        }

        [TestMethod]
        public void ResetPasswordThrowsIfCodeIsInvalid()
        {
            _mockUserManager.Setup(m => m.FindByNameAsync(It.IsAny<string>())).ReturnsAsync(GetApplicationUser());
            _mockCodeService.Setup(m => m.ValidateCode(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(false);
            
            Func<Task> func = async () => await _sut.ResetPassword(GetResetPasswordContract());

            func.Should().Throw<InvalidOperationException>().WithMessage(ApiMessages.InvalidCredentials);
        }

        [TestMethod]
        public void ResetPasswordThrowsIfResetPasswordFails()
        {
            _mockUserManager.Setup(m => m.FindByNameAsync(It.IsAny<string>())).ReturnsAsync(GetApplicationUser);
            _mockCodeService.Setup(m => m.ValidateCode(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(true);
            _mockUserManager.Setup(u => u.GeneratePasswordResetTokenAsync(It.IsAny<ApplicationUser>())).ReturnsAsync(string.Empty);
            _mockUserManager.Setup(u => 
                u.ResetPasswordAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(IdentityResult.Failed(new IdentityError()));

            Func<Task> func = async () => await _sut.ResetPassword(GetResetPasswordContract());

            func.Should().Throw<InvalidOperationException>().WithMessage(ApiMessages.InvalidCredentials);

            _mockCodeService.Verify(c => c.ValidateCode(It.IsAny<string>(), It.IsAny<string>()), Times.Once);
            _mockUserManager.Verify(c => c.GeneratePasswordResetTokenAsync(It.IsAny<ApplicationUser>()), Times.Once);
        }

        [TestMethod]
        public async Task ResetPasswordRunsSuccessfully()
        {
            _mockUserManager.Setup(m => m.FindByNameAsync(It.IsAny<string>())).ReturnsAsync(GetApplicationUser);
            _mockCodeService.Setup(m => m.ValidateCode(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(true);
            _mockUserManager.Setup(u => u.GeneratePasswordResetTokenAsync(It.IsAny<ApplicationUser>())).ReturnsAsync(string.Empty);
            _mockUserManager.Setup(u =>
                u.ResetPasswordAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(IdentityResult.Success);

            await _sut.ResetPassword(GetResetPasswordContract());

            _mockCodeService.Verify(c => c.ValidateCode(It.IsAny<string>(), It.IsAny<string>()), Times.Once);
            _mockUserManager.Verify(c => c.GeneratePasswordResetTokenAsync(It.IsAny<ApplicationUser>()), Times.Once);
            _mockUserManager.Verify(c => c.ResetPasswordAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>(), It.IsAny<string>()), Times.Once);
        }

        #endregion

        private static ApplicationUser GetApplicationUser() => new ApplicationUser
        {
            Email = _testEmail
        };

        private static ResetPasswordContract GetResetPasswordContract() => new ResetPasswordContract
        {
            Code = _testCode,
            NewPassword = _testPass,
            UserName = _userName
        };

        private static IEnumerable<object[]> GetInvalidResetPasswordContracts()
        {
            yield return new object[]
            {
                null
            };

            yield return new object[]
            { 
                new ResetPasswordContract
                {
                    NewPassword = string.Empty,
                    Code = _testCode,
                    UserName = _userName
                } 
            };

            yield return new object[]
            { 
                new ResetPasswordContract
                {
                    NewPassword = _testPass,
                    Code = string.Empty,
                    UserName = _userName
                } 
            };

            yield return new object[]
            { 
                new ResetPasswordContract
                {
                    NewPassword = _testPass,
                    Code = _testCode,
                    UserName = string.Empty
                } 
            };
        }

        private static Mock<UserManager<ApplicationUser>> GetMockUserManager()
        {
            var userStore = new Mock<IUserStore<ApplicationUser>>();

            return new Mock<UserManager<ApplicationUser>>(userStore.Object, null, null, null, null, null, null, null, null);
        }

        private static PersistedCode GetResetCode() => new PersistedCode
        {
            Code = _testCode,
            Id = _testGuid,
            Email = _testEmail,
            Expiration = DateTime.Now.AddDays(1)
        };
    }
}
