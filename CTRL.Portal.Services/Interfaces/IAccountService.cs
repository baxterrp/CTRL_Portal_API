using CTRL.Portal.Common.Contracts;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CTRL.Portal.Services.Interfaces
{
    public interface IAccountService
    {
        Task<Account> AddAccount(CreateAccountContract createAccountContract);
        Task<IEnumerable<Account>> GetAccounts(string userName);
        Task InviteUser(AccountInvitation accountInvitation);
        Task AcceptInvite(AcceptInvitation acceptInvitation);
    }
}
