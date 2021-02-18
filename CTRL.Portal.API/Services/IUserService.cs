using CTRL.Portal.API.Contracts;
using CTRL.Portal.Data.DTO;
using System.Threading.Tasks;

namespace CTRL.Portal.API.Services
{
    public interface IUserService
    {
        Task ResetPassword(ResetPasswordContract resetPasswordContract);
        Task DeleteUser(string userName);
        Task SaveCode(PersistCode persistCode);
        //Task SavePersistCode(string email);
        Task<PersistCode> SavePersistCode(string email);
    }
}
