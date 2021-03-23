using CTRL.Portal.Common.Contracts;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CTRL.Portal.Services.Interfaces
{
    public interface IBusinessEntityService
    {
        Task<BusinessEntity> AddBusinessEntity(CreateBusinessEntityContract createAccountContract);
        Task<IEnumerable<BusinessEntity>> GetBusinessEntities(string userName);
        Task InviteUser(BusinessEntityInvititation accountInvitation);
        Task AcceptInvite(AcceptInvitation acceptInvitation);
        Task CreateSubscription(SubscriptionContract subscriptionContract);
    }
}
