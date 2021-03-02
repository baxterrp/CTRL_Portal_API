namespace CTRL.Portal.API.Contracts
{
    public class ResetPasswordEmailContract : EmailContract
    {
        public string ResetCode { get; set; }
    }
}
