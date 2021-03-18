using CTRL.Portal.Services.Implementation;
using Microsoft.AspNetCore.Identity;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace CTRL.Portal.API.UnitTests
{
    [TestClass]
    public class IdentityUtilityManagerUnitTests
    {
        private readonly Mock<RoleManager<IdentityRole>> _mockRoleManager;

        public IdentityUtilityManagerUnitTests()
        {
            _mockRoleManager = GetMockRoleManager();
        }

        [TestMethod]
        public void SeedRolesExecutesCreateAsyncForEveryNonExistingRole()
        {
            _mockRoleManager.Setup(manager => manager.RoleExistsAsync(It.IsAny<string>())).ReturnsAsync(false);

            IdentityUtilityManager.SeedRoles(_mockRoleManager.Object);

            _mockRoleManager.Verify(manager => manager.RoleExistsAsync(It.IsAny<string>()), Times.Exactly(2));
            _mockRoleManager.Verify(manager => manager.CreateAsync(It.IsAny<IdentityRole>()), Times.Exactly(2));
        }

        [TestMethod]
        public void SeedRolesDoesNotExecuteIfAllRolesExist()
        {
            _mockRoleManager.Setup(manager => manager.RoleExistsAsync(It.IsAny<string>())).ReturnsAsync(true);

            IdentityUtilityManager.SeedRoles(_mockRoleManager.Object);

            _mockRoleManager.Verify(manager => manager.RoleExistsAsync(It.IsAny<string>()), Times.Exactly(2));
            _mockRoleManager.Verify(manager => manager.CreateAsync(It.IsAny<IdentityRole>()), Times.Never);
        }

        private static Mock<RoleManager<IdentityRole>> GetMockRoleManager()
        {
            var roleStore = new Mock<IRoleStore<IdentityRole>>();

            return new Mock<RoleManager<IdentityRole>>(roleStore.Object, null, null, null, null);
        }
    }
}
