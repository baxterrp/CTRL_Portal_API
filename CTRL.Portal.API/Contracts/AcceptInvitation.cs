using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CTRL.Portal.API.Contracts
{
    public class AcceptInvitation
    {
        public string Email { get; set; }
        public string Code { get; set; }
        public string UserName { get; set; }
    }
}
