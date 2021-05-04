using CTRL.Portal.Common.Contracts;
using CTRL.Portal.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace CTRL.Portal.API.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class SubscriptionController : ControllerBase
    {
        private readonly IBusinessEntityService _businessEntityService;

        public SubscriptionController(IBusinessEntityService accountService)
        {
            _businessEntityService = accountService ?? throw new ArgumentNullException(nameof(accountService));
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateSubscription([FromBody]SubscriptionContract subscriptionContract)
        {
            await _businessEntityService.CreateSubscription(subscriptionContract);

            return Ok();
        }

        [HttpPost("addSubscriptionModule")]
        public async Task<IActionResult> AddModule([FromBody]AddSubscriptionModuleContract moduleContract)
        {
            await _businessEntityService.AddModuleToSubscription(moduleContract);

            return Ok();
        }

        [HttpGet("modules")]
        public async Task<IActionResult> GetAllModules()
        {
            var allModules = await _businessEntityService.GetAllModules();

            return Ok(allModules);
        }
    }
}
