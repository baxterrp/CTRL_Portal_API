﻿using CTRL.Portal.Common.Contracts;
using CTRL.Portal.Services.Interfaces;
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

        public AuthenticationController(IAuthenticationService authenticationService)
        {
            _authenticationService = authenticationService ?? throw new ArgumentNullException(nameof(authenticationService));
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

        [HttpPost("activate")]
        public async Task<IActionResult> Activate(UserAccountActivationContract userAccountActivationContract)
        {
            await _authenticationService.ActivateUserAccount(userAccountActivationContract);

            return Ok();
        }
    }
}
