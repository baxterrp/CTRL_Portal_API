using CTRL.Portal.Data.Configuration;
using CTRL.Portal.Data.Constants;
using CTRL.Portal.Data.DataExceptions;
using CTRL.Portal.Data.DTO;
using Dapper;
using System;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace CTRL.Portal.Data.Repositories
{
    public class AccountCodeRepository : IAccountCodeRepository
    {
        private readonly DatabaseConfiguration _databaseConfiguration;

        public AccountCodeRepository(DatabaseConfiguration databaseConfiguration)
        {
            _databaseConfiguration = databaseConfiguration ?? throw new ArgumentNullException(nameof(databaseConfiguration));
        }
        public async Task<AccountCode> GetAccountId(string code)
        {
            try
            {
                using var connection = new SqlConnection(_databaseConfiguration.ConnectionString);

                var accountCode = await connection.QuerySingleAsync<AccountCode>(SqlQueries.GetAccountCodeByCode, new { Code = code });

                if (accountCode is null)
                {
                    throw new NullReferenceException($"No account Id found for {code}");
                }

                return accountCode;
            }
            catch
            {

                return null;
            }
        }

        public async Task SaveAccountCode(AccountCode accountCode)
        {
            using var connection = new SqlConnection(_databaseConfiguration.ConnectionString);
            await connection.ExecuteAsync(SqlQueries.AddAccountCode, new
            {
                Id = accountCode.Id,
                AccountId = accountCode.AccountId,
                Code = accountCode.Code,
                Accepted = accountCode.Accepted
            });
        }

        public async Task UpdateCodeStatus(string code)
        {
            using var connection = new SqlConnection(_databaseConfiguration.ConnectionString);

            await connection.ExecuteAsync(SqlQueries.UpdateCodeStatus, new { Code = code });
        }
    }
}
