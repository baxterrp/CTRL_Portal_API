using System.ComponentModel.DataAnnotations;

namespace CTRL.Portal.Common.Contracts
{
    public class UserAccountActivationContract
    {
        [Required]
        public string UserName { get; set; }

        [Required]
        public string Code { get; set; }
    }
}
