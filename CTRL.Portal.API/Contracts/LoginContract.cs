using System.ComponentModel.DataAnnotations;

namespace CTRL.Portal.API.Contracts
{
    public class LoginContract
    {
        [Required]
        public string UserName { get; set; }

        [Required]
        public string Password { get; set; }
    }
}
