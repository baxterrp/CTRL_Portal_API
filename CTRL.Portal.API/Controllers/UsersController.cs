using CTRL.Portal.API.APIConstants;
using CTRL.Portal.API.Contracts;
using CTRL.Portal.API.Exceptions;
using CTRL.Portal.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace CTRL.Portal.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;

        public UsersController(IUserService userService)
        {
            _userService = userService ?? throw new ArgumentNullException(nameof(userService));
        }

        [HttpPut("resetpass")]
        public async Task<IActionResult> ResetPassword([FromBody]ResetPasswordContract resetPasswordContract)
        {
            ValidateUserName(resetPasswordContract.UserName);

            await _userService.ResetPassword(resetPasswordContract);

            return Ok();
        }

        [HttpDelete("{userName}")]
        public async Task<IActionResult> DeleteUser([FromRoute]string userName)
        {
            ValidateUserName(userName);

            await _userService.DeleteUser(userName);

            return Ok();
        }

        private void ValidateUserName(string userName)
        {
            if (string.IsNullOrWhiteSpace(userName)) throw new ArgumentNullException(userName);

            if (!Request.Headers.TryGetValue("Authorization", out var token)) 
                throw new InvalidLoginAttemptException(ApiMessages.InvalidCredentials);

            var parsedToken = new JwtSecurityTokenHandler().ReadJwtToken(token[0].Split(" ")[1]);

            var expectedUserName = 
                parsedToken.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name && c.Value == userName)?.Value ?? string.Empty;

            if (expectedUserName != userName) throw new InvalidLoginAttemptException(ApiMessages.InvalidCredentials);
        }
    }
}
