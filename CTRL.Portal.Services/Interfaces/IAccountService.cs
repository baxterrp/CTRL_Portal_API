using CTRL.Portal.Common.Contracts;
using CTRL.Portal.Data.DTO;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CTRL.Portal.Services.Interfaces
{
    public interface IAccountService
    {
        Task<AccountDto> AddAccount(CreateAccountContract createAccountContract);
        Task<IEnumerable<AccountDto>> GetAccounts(string userName);
        Task InviteUser(AccountInvitation accountInvitation);
        Task AcceptInvite(AcceptInvitation acceptInvitation);
    }
}
