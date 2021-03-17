namespace CTRL.Portal.Common.Contracts
{
    public class ResetPasswordEmailContract : EmailContract
    {
        public string ResetCode { get; set; }
    }
}
