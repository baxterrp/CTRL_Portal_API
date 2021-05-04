using CTRL.Portal.Data.DTO;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CTRL.Portal.Data.Repositories
{
    public interface IBusinessEntityRepository
    {
        Task AddBusinessEntity(string userName, BusinessEntityDto business);
        Task<IEnumerable<BusinessEntityDto>> GetAllBusinessEntitiesByUser(string userName);
        Task<BusinessEntityDto> GetAccountById(string accountId);
        Task AddUserToAccount(string userName, string accountId);
        Task CreateSubscription(SubscriptionDto subscriptionDto);
        Task AddSubscriptionModule(SubscriptionModuleDto subscriptionModuleDto);
        Task<IEnumerable<ModuleDto>> GetAllModules();
    }
}
