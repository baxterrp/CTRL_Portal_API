using CTRL.Portal.Common.Constants;
using CTRL.Portal.Common.Contracts;
using CTRL.Portal.Data.DTO;
using CTRL.Portal.Data.Repositories;
using CTRL.Portal.Services.Constants;
using CTRL.Portal.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CTRL.Portal.Services.Implementation
{
    public class BusinessEntityService : IBusinessEntityService
    {
        private readonly IBusinessEntityRepository _businessEntityRepository;
        private readonly ICodeService _codeService;
        private readonly IEmailProvider _emailProvider;
        private readonly string _senderDomain;
        private readonly IBusinessEntityCodeRepository _accountCodeRepository;
        private readonly ICodeRepository _codeRepository;

        public BusinessEntityService(IBusinessEntityRepository businessEntityRepository, ICodeService codeService, IEmailProvider emailProvider, IBusinessEntityCodeRepository accountCodeRepository, ICodeRepository codeRepository, string senderUrl)
        {
            _businessEntityRepository = businessEntityRepository ?? throw new ArgumentNullException(nameof(businessEntityRepository));
            _codeService = codeService ?? throw new ArgumentNullException(nameof(codeService));
            _emailProvider = emailProvider ?? throw new ArgumentNullException(nameof(emailProvider));
            _senderDomain = !string.IsNullOrWhiteSpace(senderUrl) ? senderUrl : throw new ArgumentNullException(nameof(senderUrl));
            _accountCodeRepository = accountCodeRepository ?? throw new ArgumentNullException(nameof(accountCodeRepository));
            _codeRepository = codeRepository ?? throw new ArgumentNullException(nameof(codeRepository));
        }

        public async Task<BusinessEntity> AddBusinessEntity(CreateBusinessEntityContract createAccountContract)
        {
            var accountId = Guid.NewGuid().ToString();

            await _businessEntityRepository.AddBusinessEntity(createAccountContract.UserName, new BusinessEntityDto
            {
                Id = accountId,
                Name = createAccountContract.Name
            });

            return new BusinessEntity
            {
                Id = accountId,
                Name = createAccountContract.Name
            };
        }

        public async Task CreateSubscription(SubscriptionContract subscriptionContract)
        {
            if (subscriptionContract is null)
            {
                throw new ArgumentNullException(nameof(subscriptionContract));
            }

            if (string.IsNullOrWhiteSpace(subscriptionContract.BusinessEntityId))
            {
                throw new ArgumentException("BusinessEntityId cannot be null or empty", nameof(subscriptionContract.BusinessEntityId));
            }

            if (string.IsNullOrWhiteSpace(subscriptionContract.Name))
            {
                throw new ArgumentException("Name cannot be null or empty", nameof(subscriptionContract.Name));
            }

            var subscriptionId = Guid.NewGuid().ToString();

            var subscriptionDto = new SubscriptionDto
            {
                Id = subscriptionId,
                BusinessEntityId = subscriptionContract.BusinessEntityId,
                Name = subscriptionContract.Name
            };

            await _businessEntityRepository.CreateSubscription(subscriptionDto);
        }

        public async Task<IEnumerable<BusinessEntity>> GetBusinessEntities(string userName)
        {
            try
            {
                return (await _businessEntityRepository.GetAllBusinessEntitiesByUser(userName) ?? new List<BusinessEntityDto>()).Select(a => new BusinessEntity
                {
                    Id = a.Id,
                    Name = a.Name
                });
            }
            catch
            {
                return new List<BusinessEntity>();
            }
        }

        public async Task InviteUser(BusinessEntityInvititation accountInvitation)
        {
            if (accountInvitation is null)
            {
                throw new ArgumentNullException(nameof(accountInvitation));
            }

            if (string.IsNullOrWhiteSpace(accountInvitation.Email) || string.IsNullOrWhiteSpace(accountInvitation.AccountId))
            {
                throw new ArgumentException("AccountId and Email must not be null or empty", nameof(accountInvitation));
            }

            var codeResponse = _codeService.SaveCode(accountInvitation.Email);

            var accountResponse = _businessEntityRepository.GetAccountById(accountInvitation.AccountId);

            List<Task> tasks = new List<Task>
            {
                codeResponse,
                accountResponse
            };

            await Task.WhenAll(tasks);

            var accountCode = new BusinessEntityCode
            {
                Id = Guid.NewGuid().ToString(),
                BusinessEntityId = accountInvitation.AccountId,
                CodeId = codeResponse.Result.Id

            };

            await _accountCodeRepository.SaveAccountCode(accountCode);

            if (tasks.All(t => t?.IsCompletedSuccessfully ?? false))
            {
                if (!string.IsNullOrWhiteSpace(accountResponse?.Result?.Name) &&
                    !string.IsNullOrWhiteSpace(codeResponse?.Result?.Code))
                {
                    await _emailProvider.SendEmail(GetInviteEmail(accountInvitation.SenderUserName, accountResponse.Result?.Name ?? string.Empty,
                        accountInvitation.Email, codeResponse?.Result?.Code ?? string.Empty));
                }
            }
        }

        public async Task AcceptInvite(AcceptInvitation acceptInvitation)
        {
            if (acceptInvitation is null)
            {
                throw new ArgumentNullException(nameof(acceptInvitation));
            }

            if (string.IsNullOrWhiteSpace(acceptInvitation.Email) || string.IsNullOrWhiteSpace(acceptInvitation.Code))
            {
                throw new ArgumentException("Code and Email must not be null or empty", nameof(acceptInvitation));
            }

            var codeIsValid = await _codeService.ValidateCode(acceptInvitation.Email, acceptInvitation.Code);

            if (!codeIsValid)
            {
                throw new InvalidOperationException(ApiMessages.InvalidCredentials);
            }
            var accountCode = await _accountCodeRepository.GetAccountCode(acceptInvitation.Code);

            var addUserResponse = _businessEntityRepository.AddUserToAccount(acceptInvitation.UserName, accountCode.BusinessEntityId);
            var codeStatusResponse = _accountCodeRepository.UpdateCodeStatus(accountCode.CodeId);
            var codeExpirationResponse = _codeRepository.UpdateCodeExpiration(accountCode.CodeId);

            List<Task> tasks = new List<Task>
            {
                addUserResponse,
                codeStatusResponse,
                codeExpirationResponse
            };

            await Task.WhenAll(tasks);
        }

        private AccountInviteEmailContract GetInviteEmail(string sender, string accountName, string email, string code) => new AccountInviteEmailContract
        {
            Header = $"You've been invited to {accountName.ToUpper()}",
            Name = email,
            Recipient = email,
            ViewName = EmailTemplateNames.InviteToAccount,
            AccountName = accountName,
            AcceptInviteCode = code,
            SenderUserName = sender,
            SenderUrl = string.Format($"{_senderDomain}{GeneralConstants.AcceptInviteUrl}", code)
        };
    }
}
