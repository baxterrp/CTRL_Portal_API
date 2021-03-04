namespace CTRL.Portal.API.Contracts
{
    public class AccountInviteEmailContract : EmailContract
    {
        public string SenderUserName { get; set; }
        public string SenderUrl { get; set; }
        public string AccountName { get; set; }
        public string AcceptInviteCode { get; set; }
    }
}
