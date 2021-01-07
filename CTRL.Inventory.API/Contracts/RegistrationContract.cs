﻿using System.ComponentModel.DataAnnotations;

namespace CTRL.Inventory.API.Contracts
{
    public class RegistrationContract
    {
        [Required]
        public string UserName { get; set; }

        [Required]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }
    }
}
