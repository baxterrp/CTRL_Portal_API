using CTRL.Portal.Data.Configuration;
using CTRL.Portal.Data.Constants;
using CTRL.Portal.Data.DTO;
using Dapper;
using System;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace CTRL.Portal.Data.Repositories
{
    public class UserSettingsRepository : IUserSettingsRepository
    {
        private readonly DatabaseConfiguration _databaseConfiguration;

        public UserSettingsRepository(DatabaseConfiguration databaseConfiguration)
        {
            _databaseConfiguration = databaseConfiguration ?? throw new ArgumentNullException(nameof(databaseConfiguration));
        }

        public async Task<UserSettings> GetUserSettings(string userName)
        {
            using var connection = new SqlConnection(_databaseConfiguration.ConnectionString);
            return (await connection.QueryAsync<UserSettings>(SqlQueries.GetUserSettings, new { UserName = userName }))
                ?.FirstOrDefault() 
                ?? new UserSettings();
        }

        public async Task SaveSettings(UserSettings userSettings)
        {
            using var connection = new SqlConnection(_databaseConfiguration.ConnectionString);
            await connection.ExecuteAsync(SqlQueries.SaveUserSettings, new { UserName = userSettings.UserName, Theme = userSettings.Theme });
        }
    }
}
