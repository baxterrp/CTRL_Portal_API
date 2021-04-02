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
        public async Task InviteUserSendsAccountCodeEmail()
        {
            var accountCode = new BusinessEntityCode
            {
                Id = Guid.NewGuid().ToString(),
                BusinessEntityId = "testEntityId",
                CodeId = "testCodeId"

            };

            _mockBusinessEntityCodeRepository.Setup(r => r.SaveAccountCode(accountCode));
            _mockEmailProvider.Verify(e => e.SendEmail(It.IsAny<EmailContract>()));

            var task = _sut.InviteUser(GetAccountInvitation());
            await task;

            _mockBusinessEntityCodeRepository.Verify(r => r.SaveAccountCode(accountCode), Times.Once);
            _mockEmailProvider.Verify(e => e.SendEmail(It.IsAny<EmailContract>()), Times.Once);

            task.IsCompletedSuccessfully.Should().BeTrue();

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
        public void AcceptInviteAddsUserToAccount()
        {

        }

        [TestMethod]
        public void AcceptInviteUpdatesStatusOfCode()
        {

        }

        public BusinessEntityInvititation GetAccountInvitation()
        {
            BusinessEntityInvititation accountInvitation = new BusinessEntityInvititation()
            {
                SenderUserName = "testSenderName",
                AccountId = "testAccountId",
                Email = "test@email.com"
            };

            return accountInvitation;
        }
    }
}
