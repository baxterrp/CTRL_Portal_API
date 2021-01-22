﻿using System.ComponentModel.DataAnnotations;

namespace CTRL.Portal.API.Contracts
{
    public class ResetPasswordContract
    {
        [Required]
        public string UserName { get; set; }

        [Required]
        public string NewPassword { get; set; }
    }
}
