using CTRL.Portal.API.Contracts;
using CTRL.Portal.Data.DTO;
using System.Threading.Tasks;

namespace CTRL.Portal.API.Services
{
    public interface IUserService
    {
        Task ResetPassword(ResetPasswordContract resetPasswordContract);
        Task DeleteUser(string userName);
      
    }
}
