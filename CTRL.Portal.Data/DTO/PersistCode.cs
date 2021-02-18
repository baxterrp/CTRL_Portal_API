using System;

namespace CTRL.Portal.Data.DTO
{
    public class PersistCode : BaseDto
    {
        public string Email { get; set; }
        public DateTime Expiration { get; set; }
        public string Id { get; set; }
        public string ResetCode { get; set; }
    }
}
