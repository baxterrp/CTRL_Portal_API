using CTRL.Portal.Data.DTO;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CTRL.Portal.Data.Repositories
{
    public interface IAccountRepository
    {
        Task AddAccount(string userName, AccountDisplay account);
        Task<IEnumerable<AccountDisplay>> GetAllAccountsByUser(string userName);
        Task<AccountDisplay> GetAccountById(string accountId);
    }
}
