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
    public class CodeRepository : ICodeRepository
    {
        private readonly DatabaseConfiguration _databaseConfiguration;

        public CodeRepository(DatabaseConfiguration databaseConfiguration)
        {
            _databaseConfiguration = databaseConfiguration ?? throw new ArgumentNullException(nameof(databaseConfiguration));
        }

        public async Task<PersistedCodeDto> GetCode(string code, string email)
        {
            try
            {
                using var connection = new SqlConnection(_databaseConfiguration.ConnectionString);

                var persistedCode = await connection.QuerySingleAsync<PersistedCodeDto>(SqlQueries.GetCode, new { Code = code, Email = email });

                if (persistedCode is null)
                {
                    throw new ResourceNotFoundException($"No code found with value {code}");
                }

                return persistedCode;
            }
            catch
            {
                return null;
            }
        }

        public async Task SaveCode(PersistedCodeDto persistCode)
        {
            using var connection = new SqlConnection(_databaseConfiguration.ConnectionString);
            await connection.ExecuteAsync(SqlQueries.AddCode, persistCode);
        }

        public async Task UpdateCodeExpiration(string codeId)
        {
            using var connection = new SqlConnection(_databaseConfiguration.ConnectionString);
            await connection.ExecuteAsync(SqlQueries.UpdateCodeExpiration, new { Id = codeId, Expiration = DateTime.Now.AddHours(-1) }) ;
        }
    }
}
