using CTRL.Portal.Common.Contracts;
using CTRL.Portal.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace CTRL.Portal.API.Controllers
{
    //[Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class BusinessEntityController : ControllerBase
    {
        private readonly IBusinessEntityService _businessEntityService;

        public BusinessEntityController(IBusinessEntityService businessEntityService)
        {
            _businessEntityService = businessEntityService ?? throw new ArgumentNullException(nameof(businessEntityService));
        }

        [HttpPost]
        public async Task<IActionResult> CreateBusinessEntity([FromBody]CreateBusinessEntityContract createBusinessEntityContract)
        {
            var business = await _businessEntityService.AddBusinessEntity(createBusinessEntityContract);

            return Ok(business);
        }

        [HttpPost("invite")]
        public async Task<IActionResult> InviteUser([FromBody]BusinessEntityInvititation businessEntityInvititation)
        {
            await _businessEntityService.InviteUser(businessEntityInvititation);

            return Ok();
        }

        [HttpPost("acceptInvite")]
        public async Task<IActionResult> AcceptInvite([FromBody]AcceptInvitation acceptInvitation)
        {
            await _businessEntityService.AcceptInvite(acceptInvitation);

            return Ok();
        }
    }
}
