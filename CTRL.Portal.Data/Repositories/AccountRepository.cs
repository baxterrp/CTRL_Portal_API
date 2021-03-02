using CTRL.Portal.Data.Configuration;
using CTRL.Portal.Data.Constants;
using CTRL.Portal.Data.DataExceptions;
using CTRL.Portal.Data.DTO;
using Dapper;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace CTRL.Portal.Data.Repositories
{
    public class AccountRepository : IAccountRepository
    {
        private readonly DatabaseConfiguration _databaseConfiguration;

        public AccountRepository(DatabaseConfiguration databaseConfiguration)
        {
            _databaseConfiguration = databaseConfiguration ?? throw new ArgumentNullException(nameof(databaseConfiguration));
        }

        public async Task AddAccount(string userName, AccountDisplay account)
        {
            using var connection = new SqlConnection(_databaseConfiguration.ConnectionString);

            await connection.ExecuteAsync(SqlQueries.AddAccountQuery, new { account.Id, account.Name });
            await connection.ExecuteAsync(SqlQueries.AddAccountToUser, new { UserName = userName, AccountId = account.Id });
        }

        public async Task<AccountDisplay> GetAccountById(string accountId)
        {
            using var connection = new SqlConnection(_databaseConfiguration.ConnectionString);

            return await connection.QuerySingleAsync<AccountDisplay>(SqlQueries.GetAccountById, new { Id = accountId });
        }

        public async Task<IEnumerable<AccountDisplay>> GetAllAccountsByUser(string userName)
        {
            using var connection = new SqlConnection(_databaseConfiguration.ConnectionString);
            var accounts = await connection.QueryAsync<AccountDisplay>(SqlQueries.GetAllAccountsQuery, new { UserName = userName });

            if (!accounts?.Any() ?? true) throw new ResourceNotFoundException($"No accounts found with userName : {userName}");

            return accounts;
        }

        public async Task AddUserToAccount(string userName, string accountId)
        {
            using var connection = new SqlConnection(_databaseConfiguration.ConnectionString);

            await connection.ExecuteAsync(SqlQueries.AddAccountToUser, new { UserName = userName, AccountId = accountId });
        }
    }
}
