using CTRL.Portal.Data.DTO;
using System.Collections.Generic;

namespace CTRL.Portal.API.Contracts
{
    public class AuthenticationResponseContract : ApiResponseContract
    {
        public string Token { get; set; }
        public string UserName { get; set; }
        public IEnumerable<AccountDisplay> Accounts { get; set; }
    }
}
