using CTRL.Authentication.Exceptions;
using CTRL.Portal.Data.Configuration;
using CTRL.Portal.Data.Constants;
using CTRL.Portal.Data.DTO;
using Dapper;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace CTRL.Portal.Data.Repositories
{
    public class BusinessEntityRepository : IBusinessEntityRepository
    {
        private readonly DatabaseConfiguration _databaseConfiguration;

        public BusinessEntityRepository(DatabaseConfiguration databaseConfiguration)
        {
            _databaseConfiguration = databaseConfiguration ?? throw new ArgumentNullException(nameof(databaseConfiguration));
        }

        public async Task AddBusinessEntity(string userName, BusinessEntityDto business)
        {
            using var connection = new SqlConnection(_databaseConfiguration.ConnectionString);

            await connection.ExecuteAsync(SqlQueries.AddBusinessEntity, new { business.Id, business.Name });
            await connection.ExecuteAsync(SqlQueries.AddBusinessEntityToUser, new { UserName = userName, BusinessEntityId = business.Id });
        }

        public async Task<BusinessEntityDto> GetAccountById(string businessEntityId)
        {
            using var connection = new SqlConnection(_databaseConfiguration.ConnectionString);

            return await connection.QuerySingleAsync<BusinessEntityDto>(SqlQueries.GetBusinessEntityById, new { Id = businessEntityId });
        }

        public async Task<IEnumerable<BusinessEntityDto>> GetAllBusinessEntitiesByUser(string userName)
        {
            using var connection = new SqlConnection(_databaseConfiguration.ConnectionString);
            var accounts = await connection.QueryAsync<BusinessEntityDto>(SqlQueries.GetAllBusinessEntitiesQuery, new { UserName = userName });

            if (!accounts?.Any() ?? true) throw new ResourceNotFoundException($"No businesses found with userName : {userName}");

            return accounts;
        }

        public async Task AddUserToAccount(string userName, string businessEntityId)
        {
            using var connection = new SqlConnection(_databaseConfiguration.ConnectionString);

            await connection.ExecuteAsync(SqlQueries.AddBusinessEntityToUser, new { UserName = userName, BusinessEntityId = businessEntityId });
        }

        public async Task CreateSubscription(SubscriptionDto subscriptionDto)
        {
            using var connection = new SqlConnection(_databaseConfiguration.ConnectionString);

            await connection.ExecuteAsync(SqlQueries.AddSubscription, subscriptionDto);
        }

        public async Task AddSubscriptionModule(SubscriptionModuleDto subscriptionModuleDto)
        {
            using var connection = new SqlConnection(_databaseConfiguration.ConnectionString);

            await connection.ExecuteAsync(SqlQueries.AddModuleToSubscription, subscriptionModuleDto);
        }
    }
}
