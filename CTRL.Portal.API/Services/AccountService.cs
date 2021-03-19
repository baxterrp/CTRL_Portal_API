using CTRL.Portal.API.APIConstants;
using CTRL.Portal.API.Contracts;
using CTRL.Portal.Data.DTO;
using CTRL.Portal.Data.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CTRL.Portal.API.Services
{
    public class AccountService : IAccountService
    {
        private readonly IAccountRepository _accountRepository;
        private readonly ICodeService _codeService;
        private readonly IEmailProvider _emailProvider;
        private readonly string _senderDomain;

        public AccountService(IAccountRepository accountRepository, ICodeService codeService, IEmailProvider emailProvider, string senderUrl)
        {
            _accountRepository = accountRepository ?? throw new ArgumentNullException(nameof(accountRepository));
            _codeService = codeService ?? throw new ArgumentNullException(nameof(codeService));
            _emailProvider = emailProvider ?? throw new ArgumentNullException(nameof(emailProvider));
            _senderDomain = !string.IsNullOrWhiteSpace(senderUrl) ? senderUrl : throw new ArgumentNullException(nameof(senderUrl));
        }

        public async Task<AccountDisplay> AddAccount(CreateAccountContract createAccountContract)
        {
            var accountId = Guid.NewGuid().ToString();

            await _accountRepository.AddAccount(createAccountContract.UserName, new AccountDisplay
            {
                Id = accountId,
                Name = createAccountContract.Name
            });

            return new AccountDisplay
            {
                Id = accountId,
                Name = createAccountContract.Name
            };
        }

        public Task CreateSubscription(SubscriptionContract subscriptionContract)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<AccountDisplay>> GetAccounts(string userName)
        {
            try
            {
                return await _accountRepository.GetAllAccountsByUser(userName) ?? new List<AccountDisplay>();
            }
            catch
            {
                return new List<AccountDisplay>();
            }
        }

        public async Task InviteUser(AccountInvitation accountInvitation)
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

            var accountResponse = _accountRepository.GetAccountById(accountInvitation.AccountId);

            List<Task> tasks = new List<Task>
            {
                codeResponse,
                accountResponse
            };

            await Task.WhenAll(tasks);

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

        private AccountInviteEmailContract GetInviteEmail(string sender, string accountName, string email, string code)
            => new AccountInviteEmailContract
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
