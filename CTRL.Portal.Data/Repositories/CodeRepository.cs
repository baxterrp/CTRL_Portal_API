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
    public class CodeRepository : ICodeRepository
    {
        private readonly DatabaseConfiguration _databaseConfiguration;

        public CodeRepository(DatabaseConfiguration databaseConfiguration)
        {
            _databaseConfiguration = databaseConfiguration ?? throw new ArgumentNullException(nameof(databaseConfiguration));
        }
    
        public async Task SaveCode(PersistedCode persistCode)
        {
            using var connection = new SqlConnection(_databaseConfiguration.ConnectionString);
            await connection.ExecuteAsync(SqlQueries.AddCode, new {Email = persistCode.Email, Expiration = persistCode.Expiration, resetCode = persistCode.Code}); 
        }

    }
}
