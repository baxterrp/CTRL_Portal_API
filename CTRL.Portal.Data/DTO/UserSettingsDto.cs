namespace CTRL.Portal.Data.DTO
{
    public class UserSettingsDto : BaseDto
    {
        public string UserName { get; set; }
        public string Theme { get; set; }
        public string DefaultBusinessEntity { get; set; }
        public bool IsActive { get; set; }
    }
}
