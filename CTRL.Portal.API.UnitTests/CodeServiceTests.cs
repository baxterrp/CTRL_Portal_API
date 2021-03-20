using CTRL.Portal.Data.DTO;
using CTRL.Portal.Data.Repositories;
using CTRL.Portal.Services.Configuration;
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
    public class CodeServiceTests
    {
        private readonly Mock<ICodeRepository> _mockCodeRepository;
        private readonly Mock<IUtilityManager> _mockUtilityManager;
        private readonly CodeConfiguration _codeConfiguration;
        private readonly ICodeService _sut;

        private static readonly string _code = "ABC123";
        private static readonly string _email = "test@test.com";

        public CodeServiceTests()
        {
            _mockCodeRepository = new Mock<ICodeRepository>();
            _mockUtilityManager = new Mock<IUtilityManager>();

            _codeConfiguration = new CodeConfiguration
            {
                Expires = "366:0:0:0",
                Length = 6,
                Pattern = _code
            };

            _sut = new CodeService(_codeConfiguration, _mockCodeRepository.Object, _mockUtilityManager.Object);
        }

        [TestMethod]
        public void SaveCodeThrowsIfEmailIsInvalid()
        {
            var email = string.Empty;

            Func<Task> func = async () => await _sut.SaveCode(email);

            func.Should().Throw<ArgumentNullException>().And.ParamName.Should().Be("email");
        }

        [TestMethod]
        public async Task SaveCodeReturnsValidCode()
        {
            _mockUtilityManager.Setup(u => u.GenerateCode()).Returns(_code);
            _mockCodeRepository.Setup(c => c.SaveCode(It.IsAny<PersistedCodeDto>()));

            var result = await _sut.SaveCode(_email);

            result.Should().NotBeNull();
            result.Code.Should().Be(_code);
            result.Email.Should().Be(_email);
            result.Expiration.Should().BeAfter(DateTime.Now.AddYears(1));
            Guid.TryParse(result.Id, out Guid _).Should().BeTrue();
        }

        [TestMethod]
        public void ValidateCodeValidation()
        {
            Func<Task> func = async () => await _sut.ValidateCode(string.Empty, _code);
            func.Should().Throw<ArgumentNullException>().And.ParamName.Should().Be("email");

            func = async () => await _sut.ValidateCode(_email, string.Empty);
            func.Should().Throw<ArgumentNullException>().And.ParamName.Should().Be("code");
        }

        [TestMethod]
        public async Task ValidateCodeReturnsFalsesIfNoCodeReturned()
        {
            _mockCodeRepository.Setup(c => c.GetCode(_code, _email)).ReturnsAsync((PersistedCodeDto)null);

            var result = await _sut.ValidateCode(_email, _code);

            result.Should().BeFalse();
        }

        [TestMethod]
        public async Task ValidateCodeReturnsFalseIfIncorrectEmailReturned()
        {
            var persistedCode = new PersistedCodeDto
            {
                Code = _code,
                Id = Guid.NewGuid().ToString(),
                Email = "FakeEmail@email.com",
                Expiration = DateTime.Now.AddDays(1)
            };

            _mockCodeRepository.Setup(c => c.GetCode(_code, _email)).ReturnsAsync(persistedCode);

            var result = await _sut.ValidateCode(_email, _code);

            result.Should().BeFalse();
        }        
        
        [TestMethod]
        public async Task ValidateCodeReturnsFalseIfIncorrectCodeReturned()
        {
            var persistedCode = new PersistedCodeDto
            {
                Code = "InvalidCode",
                Id = Guid.NewGuid().ToString(),
                Email = _email,
                Expiration = DateTime.Now.AddDays(1)
            };

            _mockCodeRepository.Setup(c => c.GetCode(_code, _email)).ReturnsAsync(persistedCode);

            var result = await _sut.ValidateCode(_email, _code);

            result.Should().BeFalse();
        }        
        
        [TestMethod]
        public async Task ValidateCodeReturnsFalseIfExpiredCodeReturned()
        {
            var persistedCode = new PersistedCodeDto
            {
                Code = _code,
                Id = Guid.NewGuid().ToString(),
                Email = _email,
                Expiration = DateTime.Now.Subtract(TimeSpan.FromDays(1))
            };

            _mockCodeRepository.Setup(c => c.GetCode(_code, _email)).ReturnsAsync(persistedCode);

            var result = await _sut.ValidateCode(_email, _code);

            result.Should().BeFalse();
        }
        
        [TestMethod]
        public async Task ValidateCodeReturnsTrueIfValidCodeReturned()
        {
            var persistedCode = new PersistedCodeDto
            {
                Code = _code,
                Id = Guid.NewGuid().ToString(),
                Email = _email,
                Expiration = DateTime.Now.AddDays(1)
            };

            _mockCodeRepository.Setup(c => c.GetCode(_code, _email)).ReturnsAsync(persistedCode);

            var result = await _sut.ValidateCode(_email, _code);

            result.Should().BeTrue();
        }
    }
}
