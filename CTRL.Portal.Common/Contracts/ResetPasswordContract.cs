using System.ComponentModel.DataAnnotations;

namespace CTRL.Portal.Common.Contracts
{
    public class ResetPasswordContract
    {
        [Required]
        public string UserName { get; set; }

        [Required]
        public string NewPassword { get; set; }

        [Required]
        public string Code { get; set; }
    }
}
