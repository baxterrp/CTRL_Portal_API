using CTRL.Portal.API.Contracts;
using CTRL.Portal.Data.DTO;
using CTRL.Portal.Data.Repositories;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CTRL.Portal.API.Services
{
    public class AccountService : IAccountService
    {
        private readonly IAccountRepository _accountRepository;

        public AccountService(IAccountRepository accountRepository)
        {
            _accountRepository = accountRepository ?? throw new ArgumentNullException(nameof(accountRepository));
        }

        public async Task AddAccount(CreateAccountContract createAccountContract)
        {
            await _accountRepository.AddAccount(createAccountContract.UserName, new AccountDisplay
            {
                Id = Guid.NewGuid().ToString(),
                Name = createAccountContract.Name
            });
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
    }
}
