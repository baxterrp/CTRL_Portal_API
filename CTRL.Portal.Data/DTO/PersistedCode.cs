using System;

namespace CTRL.Portal.Data.DTO
{
    public class PersistedCode : BaseDto
    {
        public string Email { get; set; }
        public DateTime Expiration { get; set; }
        public string Code { get; set; }
    }
}
