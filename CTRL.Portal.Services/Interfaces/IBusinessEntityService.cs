﻿using CTRL.Portal.Common.Contracts;
using CTRL.Portal.Data.DTO;
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
        Task<SubscriptionDto> CreateSubscription(SubscriptionContract subscriptionContract);
        Task AddModuleToSubscription(AddSubscriptionModuleContract moduleContract);
    }
}
