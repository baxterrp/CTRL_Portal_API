using CTRL.Portal.API.Contracts;
using CTRL.Portal.Data.DTO;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CTRL.Portal.API.Services
{
    public interface IAccountService
    {
        Task<AccountDisplay> AddAccount(CreateAccountContract createAccountContract);
        Task<IEnumerable<AccountDisplay>> GetAccounts(string userName);
        Task InviteUser(AccountInvitation accountInvitation);
        Task AcceptInvite(AcceptInvitation acceptInvitation);
    }
}
