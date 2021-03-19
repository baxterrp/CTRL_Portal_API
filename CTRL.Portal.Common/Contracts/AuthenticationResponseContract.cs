using System.Collections.Generic;
using System.Net;

namespace CTRL.Portal.Common.Contracts
{
    public class AuthenticationResponseContract
    {
        public string Token { get; set; }
        public string UserName { get; set; }
        public IEnumerable<Account> Accounts { get; set; }
        public UserSettings UserSettings { get; set; }
        public HttpStatusCode Status { get; set; }
        public string Message { get; set; }
    }
}
