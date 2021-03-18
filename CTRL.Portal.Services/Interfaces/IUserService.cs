using CTRL.Portal.Common.Contracts;
using System.Threading.Tasks;

namespace CTRL.Portal.Services.Interfaces
{
    public interface IUserService
    {
        Task RequestPasswordReset(string email);
        Task ResetPassword(ResetPasswordContract resetPasswordContract);
        Task DeleteUser(string userName);
    }
}
