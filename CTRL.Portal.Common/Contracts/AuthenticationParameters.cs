namespace CTRL.Portal.Common.Contracts
{
    public class AuthenticationParameters
    {
        public string Issuer { get; set; }
        public string Audience { get; set; }
        public string Key { get; set; }
    }
}