using CTRL.Portal.Services.Configuration;
using CTRL.Portal.Services.Implementation;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CTRL.Portal.Services.UnitTests
{
    [TestClass]
    public class UtilityManagerUnitTests
    {
        [TestMethod]
        [DataRow(1)]
        [DataRow(2)]
        [DataRow(3)]
        [DataRow(4)]
        [DataRow(100)]
        public void GenerateCodeReturnsCorrectLength(int length)
        {
            //arange
            var config = new CodeConfiguration
            {
                Expires = "0:0:0:0",
                Length = length,
                Pattern = "A"
            };

            var sut = new UtilityManager(config);

            //act
            var actual = sut.GenerateCode();

            //assert
            actual.Length.Should().Be(length);
        }
        [TestMethod]
        public void GenerateCodeReturnsExpectedPattern()
        {
            //arange
            var config = new CodeConfiguration
            {
                Expires = "0:0:0:0",
                Length = 4,
                Pattern = "A"
            };

            var sut = new UtilityManager(config);
            var expectedResult = "AAAA";

            //act
            var actual = sut.GenerateCode();

            //assert
            actual.Should().Be(expectedResult);
        }

        [TestMethod]
        [DataRow("ABC")]
        [DataRow("123")]
        [DataRow("!@#")]
        public void GenerateCodeReturnsExpectedChars(string pattern)
        {
            //arange
            var config = new CodeConfiguration
            {
                Expires = "0:0:0:0",
                Length = 50,
                Pattern = pattern
            };

            var sut = new UtilityManager(config);

            //act
            var actual = sut.GenerateCode();

            //assert
            foreach (var letter in actual)
            {
                config.Pattern.Contains(letter).Should().BeTrue();
            }
        }
    }
}
