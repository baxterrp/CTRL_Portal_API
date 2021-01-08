namespace CTRL.Portal.API.Configuration
{
    public class AuthenticationConfiguration
    {
        public string ValidAudience { get; set; }
        public string ValidIssuer { get; set; }
        public string Secret { get; set; }
    }
}
