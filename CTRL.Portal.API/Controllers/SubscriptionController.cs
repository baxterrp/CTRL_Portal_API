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
        private readonly IAccountService _accountService;

        public SubscriptionController(IAccountService accountService)
        {
            _accountService = accountService ?? throw new ArgumentNullException(nameof(accountService));
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateSubscription([FromBody]SubscriptionContract subscriptionContract)
        {
            await _accountService.CreateSubscription(subscriptionContract);

            return Ok();
        }
    }
}
