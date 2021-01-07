using CTRL.Inventory.API.Contracts;
using System.Threading.Tasks;

namespace CTRL.Inventory.API.Services
{
    public interface IAuthenticationService
    {
        Task Register(RegistrationContract registrationContract);
        Task<AuthenticationResponseContract> Login(LoginContract loginContract);
    }
}
