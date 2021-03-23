using CTRL.Portal.Common.Contracts;
using System.Threading.Tasks;

namespace CTRL.Portal.Services.Interfaces
{
    public interface IAuthenticationService
    {
        Task Register(RegistrationContract registrationContract);
        Task<AuthenticationResponseContract> Login(LoginContract loginContract);
        Task ActivateUserAccount(UserAccountActivationContract userAccountActivationContract);
    }
}
