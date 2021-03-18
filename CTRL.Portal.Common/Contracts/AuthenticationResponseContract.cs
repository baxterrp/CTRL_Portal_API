using System.Collections.Generic;

namespace CTRL.Portal.Common.Contracts
{
    public class AuthenticationResponseContract : ApiResponseContract
    {
        public string Token { get; set; }
        public string UserName { get; set; }
        public IEnumerable<Account> Accounts { get; set; }
        public UserSettings UserSettings { get; set; }
    }
}
