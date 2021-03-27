using CTRL.Portal.Common.Contracts;
using CTRL.Portal.Data.DTO;
using CTRL.Portal.Data.Repositories;
using CTRL.Portal.Services.Implementation;
using CTRL.Portal.Services.Interfaces;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Threading.Tasks;

namespace CTRL.Portal.Services.UnitTests
{
    [TestClass]
    public class BusinessEntityServiceUnitTests
    {
        private readonly IBusinessEntityService _sut;
        private readonly Mock<IBusinessEntityRepository> _mockBusinessEntityRepository;
        private readonly Mock<ICodeService> _mockCodeService;
        private readonly Mock<IEmailProvider> _mockEmailProvider;
        private readonly Mock<IBusinessEntityCodeRepository> _mockBusinessEntityCodeRepository;
        private static readonly string _senderDomain = "/test/url";

        public BusinessEntityServiceUnitTests()
        {
            _mockBusinessEntityRepository = new Mock<IBusinessEntityRepository>();
            _mockCodeService = new Mock<ICodeService>();
            _mockEmailProvider = new Mock<IEmailProvider>();
            _mockBusinessEntityCodeRepository = new Mock<IBusinessEntityCodeRepository>();

            _sut = new BusinessEntityService(_mockBusinessEntityRepository.Object, _mockCodeService.Object, _mockEmailProvider.Object, _mockBusinessEntityCodeRepository.Object, _senderDomain);
        }

        [TestMethod]
        public void InviteUserThrowsIfInvalidInvitation()
        {
            BusinessEntityInvititation invite = null;

            Func<Task> func = async () => await _sut.InviteUser(invite);

            func.Should().Throw<ArgumentNullException>().And.ParamName.Should().Be("accountInvitation");
        }

        [TestMethod]
        public void InviteUserThrowsIfEmailOrAccountIdNullOrEmpty()
        {
            BusinessEntityInvititation invite = new BusinessEntityInvititation
            {
                Email = string.Empty,
                AccountId = "1234"
            };

            Func<Task> func = async () => await _sut.InviteUser(invite);

            func.Should().Throw<ArgumentException>().WithMessage("AccountId and Email must not be null or empty (Parameter 'accountInvitation')");

            invite = new BusinessEntityInvititation
            {
                Email = "test@test.com",
                AccountId = string.Empty
            };

            func = async () => await _sut.InviteUser(invite);

            func.Should().Throw<ArgumentException>().WithMessage("AccountId and Email must not be null or empty (Parameter 'accountInvitation')");
        }

        [TestMethod]
        public async Task InviteUserSendsEmail()
        {
            // Arrange
            var invite = new BusinessEntityInvititation
            {
                AccountId = Guid.NewGuid().ToString(),
                Email = "test@test.com",
                SenderUserName = "testUser"
            };

            var codeDto = new PersistedCodeDto
            {
                Id = Guid.NewGuid().ToString(),
                Code = "ABC123",
                Email = "test@test.com",
                Expiration = DateTime.Now.AddDays(1)
            };

            var businessDto = new BusinessEntityDto
            {
                Id = Guid.NewGuid().ToString(),
                Name = "TestBusiness"
            };

            _mockCodeService.Setup(c => c.SaveCode(It.IsAny<string>())).ReturnsAsync(codeDto);
            _mockBusinessEntityRepository.Setup(b => b.GetAccountById(It.IsAny<string>())).ReturnsAsync(businessDto);
            _mockBusinessEntityCodeRepository.Setup(b => b.SaveAccountCode(It.IsAny<BusinessEntityCode>()));
            _mockEmailProvider.Setup(e => e.SendEmail(It.IsAny<EmailContract>()));
            
            // Act
            await _sut.InviteUser(invite);

            // Assert
            _mockCodeService.Verify(c => c.SaveCode(It.IsAny<string>()), Times.Once);
            _mockBusinessEntityRepository.Verify(b => b.GetAccountById(It.IsAny<string>()), Times.Once);
            _mockBusinessEntityCodeRepository.Verify(b => b.SaveAccountCode(It.IsAny<BusinessEntityCode>()), Times.Once);
            _mockEmailProvider.Verify(e => e.SendEmail(It.IsAny<EmailContract>()), Times.Once);
        }
    }
}
