using CTRL.Portal.API.Contracts;
using CTRL.Portal.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace CTRL.Portal.API.Controllers
{
    //[Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class AccountController : ControllerBase
    {
        private readonly IAccountService _accountService;

        public AccountController(IAccountService accountService)
        {
            _accountService = accountService ?? throw new ArgumentNullException(nameof(accountService));
        }

        [HttpPost]
        public async Task<IActionResult> CreateAccount([FromBody]CreateAccountContract createAccountContract)
        {
            var account = await _accountService.AddAccount(createAccountContract);

            return Ok(account);
        }

        [HttpPost("invite")]
        public async Task<IActionResult> InviteUser([FromBody]AccountInvitation accountInvitation)
        {
            await _accountService.InviteUser(accountInvitation);

            return Ok();
        }

        [HttpPost("acceptInvite")]
        public async Task<IActionResult> AcceptInvite([FromBody]AcceptInvitation acceptInvitation)
        {
            await _accountService.AcceptInvite(acceptInvitation);

            return Ok();
        }
    }
}
