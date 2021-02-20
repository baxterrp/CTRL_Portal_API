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
    public class CodeRepository : ICodeRepository
    {
        private readonly DatabaseConfiguration _databaseConfiguration;

        public CodeRepository(DatabaseConfiguration databaseConfiguration)
        {
            _databaseConfiguration = databaseConfiguration ?? throw new ArgumentNullException(nameof(databaseConfiguration));
        }

        public async Task<PersistedCode> GetCode(string code)
        {
            using var connection = new SqlConnection(_databaseConfiguration.ConnectionString);

            var persistedCode = await connection.QuerySingleAsync<PersistedCode>(SqlQueries.GetCode, new { Code = code });

            if(persistedCode is null)
            {
                throw new ResourceNotFoundException($"No code found with value {code}");
            }

            return persistedCode;
        }

        public async Task SaveCode(PersistedCode persistCode)
        {
            using var connection = new SqlConnection(_databaseConfiguration.ConnectionString);
            await connection.ExecuteAsync(SqlQueries.AddCode, new {Id = persistCode.Id, Email = persistCode.Email, Expiration = persistCode.Expiration, Code = persistCode.Code}); 
        }

    }
}
