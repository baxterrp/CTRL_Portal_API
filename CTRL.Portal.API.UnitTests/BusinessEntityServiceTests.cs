using CTRL.Portal.Common.Constants;
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
    public class BusinessEntityServiceTests
    {
        private readonly Mock<IBusinessEntityRepository> _mockBusinessEntityRepository;
        private readonly Mock<IBusinessEntityCodeRepository> _mockBusinessEntityCodeRepository;
        private readonly Mock<ICodeService> _mockCodeService;
        private readonly Mock<IEmailProvider> _mockEmailProvider;
        private readonly IBusinessEntityService _sut;
        private static readonly string _senderURL = "./test/";

        public BusinessEntityServiceTests()
        {
            _mockBusinessEntityRepository = new Mock<IBusinessEntityRepository>();
            _mockBusinessEntityCodeRepository = new Mock<IBusinessEntityCodeRepository>();
            _mockCodeService = new Mock<ICodeService>();
            _mockEmailProvider = new Mock<IEmailProvider>();

            _sut = new BusinessEntityService(
                _mockBusinessEntityRepository.Object, _mockCodeService.Object,
                _mockEmailProvider.Object, _mockBusinessEntityCodeRepository.Object, _senderURL);
        }
        [TestMethod]
        public async Task AddBusinessEntityReturnsValidBusinessEntity()
        {
            var createAccountContract = new CreateBusinessEntityContract()
            {
                Name = "testName",
                UserName = "userName"
            };

            var result = await _sut.AddBusinessEntity(createAccountContract);

            result.Should().NotBeNull();
            Guid.TryParse(result.Id, out Guid _).Should().BeTrue();
            result.Name.Should().Be("testName");
        }

        [TestMethod]
        public void CreateSubscriptionThrowsIfContractIsNull()
        {
            Func<Task> func = async () => await _sut.CreateSubscription((SubscriptionContract)null);
            func.Should().Throw<ArgumentNullException>();
        }

        [TestMethod]
        public void CreateSubscriptionThrowsIfBusinessEntityIdIsNull()
        {
            var subscriptionContract = new SubscriptionContract()
            {
                BusinessEntityId = string.Empty,
                Name = "testName"
            };

            Func<Task> func = async () => await _sut.CreateSubscription(subscriptionContract);
            func.Should().Throw<ArgumentException>().And.ParamName.Should().Be("BusinessEntityId");
        }

        [TestMethod]
        public void CreateSubscriptionThrowsIfNameIsNull()
        {
            var subscriptionContract = new SubscriptionContract()
            {
                BusinessEntityId = "testId",
                Name = string.Empty
            };

            Func<Task> func = async () => await _sut.CreateSubscription(subscriptionContract);
            func.Should().Throw<ArgumentException>().And.ParamName.Should().Be("Name");
        }

        [TestMethod]
        public async Task CreateSubscriptionCreatesValidSubscriptionDto()
        {
            var subscriptionContract = new SubscriptionContract()
            {
                BusinessEntityId = "testBusinessEntity",
                Name = "testName"
            };

            _mockBusinessEntityRepository.Setup(r => r.CreateSubscription(It.IsAny<SubscriptionDto>()));

            var task = _sut.CreateSubscription(subscriptionContract);
            await task;

            _mockBusinessEntityRepository.Verify(r => r.CreateSubscription(It.IsAny<SubscriptionDto>()), Times.Once);

            task.IsCompletedSuccessfully.Should().BeTrue();
        }

        [TestMethod]
        public void GetBusinessEntitiesReturnsValidEntities()
        {

        }

        [TestMethod]
        public void InviteUserThrowsIfAccountInvitationIsNull()
        {
            Func<Task> func = async () => await _sut.InviteUser((BusinessEntityInvititation)null);
            func.Should().Throw<ArgumentNullException>().And.ParamName.Should().Be("accountInvitation");
        }

        [TestMethod]
        public void InviteUserThrowsIfEmailIsNullOrEmpty()
        {
            BusinessEntityInvititation accountInvitation = new BusinessEntityInvititation()
            {
                SenderUserName = "testSenderName",
                AccountId = "testAccountId",
                Email = string.Empty
            };

            Func<Task> func = async () => await _sut.InviteUser(accountInvitation);
            func.Should().Throw<ArgumentException>().WithMessage("AccountId and Email must not be null or empty (Parameter 'accountInvitation')");
        }

        [TestMethod]
        public async Task InviteUserSendsEmail()
        {
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

            var accountCode = new BusinessEntityCode
            {
                Id = Guid.NewGuid().ToString(),
                BusinessEntityId = "testEntityId",
                CodeId = "testCodeId"

            };

            _mockCodeService.Setup(c => c.SaveCode(It.IsAny<string>())).ReturnsAsync(codeDto);
            _mockBusinessEntityRepository.Setup(b => b.GetAccountById(It.IsAny<string>())).ReturnsAsync(businessDto);
            _mockBusinessEntityCodeRepository.Setup(b => b.SaveAccountCode(It.IsAny<BusinessEntityCode>()));
            _mockEmailProvider.Setup(e => e.SendEmail(It.IsAny<EmailContract>()));

            await _sut.InviteUser(invite);

            _mockCodeService.Verify(c => c.SaveCode(It.IsAny<string>()), Times.Once);
            _mockBusinessEntityRepository.Verify(b => b.GetAccountById(It.IsAny<string>()), Times.Once);
            _mockBusinessEntityCodeRepository.Verify(b => b.SaveAccountCode(It.IsAny<BusinessEntityCode>()), Times.Once);
            _mockEmailProvider.Verify(e => e.SendEmail(It.IsAny<EmailContract>()), Times.Once);
        }

        [TestMethod]
        public void AcceptInviteThrowsIfAcceptInvitationIsNull()
        {
            Func<Task> func = async () => await _sut.AcceptInvite((AcceptInvitation)null);
            func.Should().Throw<ArgumentNullException>();
        }

        [TestMethod]
        public void AcceptInviteThrowsIfEmailIsNullOrEmpty()
        {
            var acceptInvitation = new AcceptInvitation()
            {
                Email = string.Empty,
                Code = "abc123",
                UserName = "testUser"
            };

            Func<Task> func = async () => await _sut.AcceptInvite(acceptInvitation);
            func.Should().Throw<ArgumentException>().WithMessage("Code and Email must not be null or empty (Parameter 'acceptInvitation')");
        }

        [TestMethod]
        public void AcceptInviteThrowsIfCodeIsNotValid()
        {
            var acceptInvitation = new AcceptInvitation()
            {
                Email = "test@test.com",
                Code = "abc123",
                UserName = "testUser"
            };

            Func<Task> func = async () => await _sut.AcceptInvite(acceptInvitation);

            _mockCodeService.Setup(m => m.ValidateCode(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(false);

            func.Should().Throw<InvalidOperationException>().WithMessage(ApiMessages.InvalidCredentials);
        }

        [TestMethod]
        public async Task AcceptInviteAddsUserToAccount()
        {
            var acceptInvitation = new AcceptInvitation()
            {
                Email = "test@test.com",
                Code = "abc123",
                UserName = "testUser"
            };

            var accountCode = new BusinessEntityCode
            {
                Id = Guid.NewGuid().ToString(),
                BusinessEntityId = "testEntityId",
                CodeId = "testCodeId"

            };


            _mockBusinessEntityRepository.Setup(e => e.AddUserToAccount(It.IsAny<string>(), It.IsAny<string>()));
            _mockCodeService.Setup(m => m.ValidateCode(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(false);

            await _sut.AcceptInvite(acceptInvitation); //cannot get past validating code

            _mockBusinessEntityRepository.Verify(e => e.AddUserToAccount(It.IsAny<string>(), It.IsAny<string>()), Times.Once());
        }

        [TestMethod]
        public void AcceptInviteUpdatesStatusOfCode()
        {

        }
    }
}
