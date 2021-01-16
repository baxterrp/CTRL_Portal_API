﻿using CTRL.Portal.API.Services;
using CTRL.Portal.Data.DTO;
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
        public async Task<IActionResult> SaveSettings([FromBody]UserSettings userSettings)
        {
            if (userSettings is null ||
                string.IsNullOrWhiteSpace(userSettings?.UserName) ||
                string.IsNullOrWhiteSpace(userSettings?.Theme))
                throw new ArgumentException(nameof(userSettings));

            await _userSettingsService.SaveSettings(userSettings);

            return Ok();
        }
    }
}
