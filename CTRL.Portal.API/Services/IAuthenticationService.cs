using CTRL.Portal.API.Contracts;
using System.Threading.Tasks;

namespace CTRL.Portal.API.Services
{
    public interface IAuthenticationService
    {
        Task Register(RegistrationContract registrationContract);
        Task<AuthenticationResponseContract> Login(LoginContract loginContract);
    }
}
