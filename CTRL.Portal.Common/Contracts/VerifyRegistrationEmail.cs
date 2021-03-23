namespace CTRL.Portal.Common.Contracts
{
    public class VerifyRegistrationEmail : EmailContract
    {
        public string VerificationLink { get; set; }
    }
}
