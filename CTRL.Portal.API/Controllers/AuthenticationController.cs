using CTRL.Portal.API.Contracts;
using CTRL.Portal.API.Services;
using CTRL.Portal.Data.DTO;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace CTRL.Portal.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthenticationController : ControllerBase
    {
        private readonly IAuthenticationService _authenticationService;
        private readonly IEmailProvider _emailProvider;
        private readonly ICodeService _codeService;

        public AuthenticationController(IAuthenticationService authenticationService, IEmailProvider emailProvider, ICodeService codeService)
        {
            _authenticationService = authenticationService ?? throw new ArgumentNullException(nameof(authenticationService));
            _emailProvider = emailProvider ?? throw new ArgumentNullException(nameof(emailProvider));
            _codeService = codeService ?? throw new ArgumentNullException(nameof(codeService));
        }
    
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegistrationContract registrationContract)
        {
            await _authenticationService.Register(registrationContract);

            return Ok();
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginContract loginContract) =>
            Ok(await _authenticationService.Login(loginContract));

        [HttpPost("emailTest")]
        public OkResult TestEmail([FromBody] EmailContract emailContract)
        {

             _emailProvider.SendEmail(emailContract);
            return Ok();
        }

        [HttpPost("savePersistCode")] 
        public async Task<IActionResult> SavePersistCode(string email)
        {
            await _codeService.SaveCode(email);

            return Ok(email);
        }

    }
}
