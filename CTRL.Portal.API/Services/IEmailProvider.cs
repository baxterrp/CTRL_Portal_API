using CTRL.Portal.API.Contracts;

namespace CTRL.Portal.API.Services
{
    public interface IEmailProvider
    {
        void SendEmail(EmailContract email);
    }
}
