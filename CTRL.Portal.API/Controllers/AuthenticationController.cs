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
        private readonly IUserService _userService;

        public AuthenticationController(IAuthenticationService authenticationService, IEmailProvider emailProvider, IUserService userService)
        {
            _authenticationService = authenticationService ?? throw new ArgumentNullException(nameof(authenticationService));
            _emailProvider = emailProvider ?? throw new ArgumentNullException(nameof(emailProvider));
            _userService = userService ?? throw new ArgumentNullException(nameof(userService));
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

        [HttpPost("savePersistCode")] //does not work
        public async Task<IActionResult> SavePersistCode([FromBody] string email)
        {

            await _userService.SavePersistCode(email);

            return Ok();
        }

        //[HttpPost("saveCode")]  //this does not work
        //public async Task<IActionResult> SendCode([FromBody] string email) =>
        //    Ok(await _userService.GenerateCode(email));

    }
}
