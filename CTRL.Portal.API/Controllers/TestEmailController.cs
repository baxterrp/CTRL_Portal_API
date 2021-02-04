using CTRL.Portal.API.Contracts;
using CTRL.Portal.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;

namespace CTRL.Portal.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class TestEmailController : ControllerBase
    {
        private readonly IEmailProvider _emailerService;

        public TestEmailController(IEmailProvider emailerService)
        {
            _emailerService = emailerService ?? throw new ArgumentNullException(nameof(emailerService));
        }

        [HttpPost("send")]
        public IActionResult SendTestEmail([FromBody] EmailContract emailContract)
        {
            _emailerService.SendEmail(emailContract);

            return Ok();
        }
    }
}
