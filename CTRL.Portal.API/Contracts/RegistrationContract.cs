using System.ComponentModel.DataAnnotations;

namespace CTRL.Portal.API.Contracts
{
    public class RegistrationContract
    {
        [Required]
        [RegularExpression(@"^\w+$", ErrorMessage = "Username must contain only letters and numbers")]
        public string UserName { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!#%*?&])[A-Za-z\d@$!#%*?&]{8,}$", 
            ErrorMessage ="Password must contain min 8 characters, upper and lower case, a number, and a special character")]
        public string Password { get; set; }
    }
}
