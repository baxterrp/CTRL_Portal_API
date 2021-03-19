using CTRL.Portal.API.Contracts;
using CTRL.Portal.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CTRL.Portal.API.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class SubscriptionController : ControllerBase
    {
        private readonly IAccountService _accountService;

        [HttpPost("create")]
        public async Task<IActionResult> CreateSubscription([FromBody]SubscriptionContract subscriptionContract)
        {
            await _accountService.CreateSubscription(subscriptionContract);

            return Ok();
        }
    }
}
