using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CTRL.Portal.Data.DTO
{
    public class AccountCode : BaseDto
    {
        public string AccountId { get; set; }
        public string Code { get; set; }
        public bool Accepted { get; set; }
    }
}
