using CTRL.Portal.Data.DTO;
using CTRL.Portal.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace CTRL.Portal.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class UserSettingsController : ControllerBase
    {
        private readonly IUserSettingsService _userSettingsService;

        public UserSettingsController(IUserSettingsService userSettingsService)
        {
            _userSettingsService = userSettingsService ?? throw new ArgumentNullException(nameof(userSettingsService));
        }

        [HttpPost]
        public async Task<IActionResult> SaveSettings([FromBody] UserSettingsDto userSettings)
        {
            if (userSettings is null ||
                string.IsNullOrWhiteSpace(userSettings?.UserName))
            {
                throw new ArgumentException(nameof(userSettings));
            }

            await _userSettingsService.SaveSettings(userSettings);

            return Ok();
        }
    }
}
