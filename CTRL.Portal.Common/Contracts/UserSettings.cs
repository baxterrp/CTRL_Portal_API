namespace CTRL.Portal.Common.Contracts
{
    public class UserSettings
    {
        public string Id { get; set; }
        public string UserName { get; set; }
        public string Theme { get; set; }
        public string DefaultAccount { get; set; }
        public bool IsActive { get; set; }
    }
}
