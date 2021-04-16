using CTRL.Authentication.Exceptions;
using CTRL.Portal.Data.Configuration;
using CTRL.Portal.Data.Constants;
using CTRL.Portal.Data.DTO;
using Dapper;
using System;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace CTRL.Portal.Data.Repositories
{
    public class BusinessEntityCodeRepository : IBusinessEntityCodeRepository
    {
        private readonly DatabaseConfiguration _databaseConfiguration;

        public BusinessEntityCodeRepository(DatabaseConfiguration databaseConfiguration)
        {
            _databaseConfiguration = databaseConfiguration ?? throw new ArgumentNullException(nameof(databaseConfiguration));
        }
        public async Task<BusinessEntityCode> GetAccountCode(string code)
        {
            try
            {
                using var connection = new SqlConnection(_databaseConfiguration.ConnectionString);

                var accountId = await connection.QuerySingleAsync<BusinessEntityCode>(SqlQueries.GetBussinessEntityCodeByCodeId, new { Code = code });

                return accountId;
            }
            catch
            {
                throw new ResourceNotFoundException($"No business Id found for {code}");
            }
        }

        public async Task SaveAccountCode(BusinessEntityCode accountCode)
        {
            using var connection = new SqlConnection(_databaseConfiguration.ConnectionString);
            await connection.ExecuteAsync(SqlQueries.AddBusinessEntityCode, accountCode);
        }

        public async Task UpdateCodeStatus(string codeId)
        {
            using var connection = new SqlConnection(_databaseConfiguration.ConnectionString);

            await connection.ExecuteAsync(SqlQueries.UpdateCodeStatus, new { CodeId = codeId });
        }
    }
}
