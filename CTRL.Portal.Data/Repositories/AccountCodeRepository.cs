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
        public async Task<AccountCode> GetAccountCode(string code)
        {
            try
            {
                using var connection = new SqlConnection(_databaseConfiguration.ConnectionString);

                var accountId = await connection.QuerySingleAsync<AccountCode>(SqlQueries.GetAccountCodeByCodeId, new { Code = code });

                return accountId;
            }
            catch
            {
                throw new ResourceNotFoundException($"No account Id found for {code}");
            }
        }

        public async Task SaveAccountCode(AccountCode accountCode)
        {
            using var connection = new SqlConnection(_databaseConfiguration.ConnectionString);
            await connection.ExecuteAsync(SqlQueries.AddAccountCode, accountCode);
        }

        public async Task UpdateCodeStatus(string codeId)
        {
            using var connection = new SqlConnection(_databaseConfiguration.ConnectionString);

            await connection.ExecuteAsync(SqlQueries.UpdateCodeStatus, new { CodeId = codeId });
        }
    }
}
