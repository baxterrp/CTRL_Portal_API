namespace CTRL.Portal.Data.DTO
{
    public class UserSettings : BaseDto
    {
        public string UserName { get; set; }
        public string Theme { get; set; }
        public string DefaultAccount { get; set; }
    }
}
