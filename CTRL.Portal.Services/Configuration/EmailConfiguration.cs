namespace CTRL.Portal.Services.Configuration
{
    public class EmailConfiguration
    {
        public string SmtpServer { get; set; }
        public int Port { get; set; }
        public string Login { get; set; }
        public string Password { get; set; }
        public string SenderUrl { get; set; }
        public string SenderName { get; set; }
    }
}
