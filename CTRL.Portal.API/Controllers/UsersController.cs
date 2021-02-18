using CTRL.Portal.API.Contracts;
using CTRL.Portal.API.Services;
using CTRL.Portal.Data.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
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
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordContract resetPasswordContract)
        {
            await _userService.ResetPassword(resetPasswordContract);

            return Ok();
        }

        [HttpDelete("{userName}")]
        public async Task<IActionResult> DeleteUser([FromRoute] string userName)
        {
            await _userService.DeleteUser(userName);

            return Ok();
        }

        [HttpPost("savePersistCode")] //does not work
        public async Task<IActionResult> SavePersistCode([FromBody] string email)
        {

            await _userService.SavePersistCode(email);

            return Ok();
        }

        //[HttpPost("saveCode")] //does not work
        //public async Task<IActionResult> SendCode([FromBody] string email)
        //{

        //    await _userService.SaveCode(persistCode);

        //        return Ok();
        //}
    }
}
