using CTRL.Portal.API.Contracts;
using System.Threading.Tasks;

namespace CTRL.Portal.API.Services
{
    public interface IUserService
    {
        Task RequestPasswordReset(string email);
        Task ResetPassword(ResetPasswordContract resetPasswordContract);
        Task DeleteUser(string userName);
    }
}
