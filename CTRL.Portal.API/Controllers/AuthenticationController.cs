using CTRL.Portal.API.Contracts;
using CTRL.Portal.API.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Net;
using System.Threading.Tasks;

namespace CTRL.Portal.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthenticationController : ControllerBase
    {
        private readonly IAuthenticationService _authenticationService;

        public AuthenticationController(IAuthenticationService authenticationService)
        {
            _authenticationService = authenticationService ?? throw new ArgumentNullException(nameof(authenticationService));
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegistrationContract registrationContract)
        {
            try
            {
                await _authenticationService.Register(registrationContract);

                return Ok(new ApiResponseContract
                {
                    Status = HttpStatusCode.OK,
                    Message = "User created successfully"
                });
            }
            catch
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new ApiResponseContract
                {
                    Status = HttpStatusCode.InternalServerError,
                    Message = "An unhandled error occured"
                });
            }
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginContract loginContract)
        {
            try
            {
                var response = await _authenticationService.Login(loginContract);

                return StatusCode((int)response.Status, response);
            }
            catch
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new ApiResponseContract
                {
                    Status = HttpStatusCode.InternalServerError,
                    Message = "An unhandled error occured"
                });
            }
        }
    }
}
