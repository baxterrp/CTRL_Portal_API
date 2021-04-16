using CTRL.Portal.Data.DTO;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CTRL.Portal.Data.Repositories
{
    public interface IAccountRepository
    {
        Task AddAccount(string userName, AccountDto account);
        Task<IEnumerable<AccountDto>> GetAllAccountsByUser(string userName);
        Task<AccountDto> GetAccountById(string accountId);
        Task AddUserToAccount(string userName, string accountId);
        Task CreateSubscription(SubscriptionDto subscriptionDto);
        Task AddSubscriptionModule(SubscriptionModuleDto subscriptionModuleDto);
    }
}
