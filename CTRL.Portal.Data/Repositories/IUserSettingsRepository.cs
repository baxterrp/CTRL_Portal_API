using CTRL.Portal.Data.DTO;
using System.Threading.Tasks;

namespace CTRL.Portal.Data.Repositories
{
    public interface IUserSettingsRepository
    {
        Task SaveSettings(UserSettings userSettings);
        Task<UserSettings> GetUserSettings(string userName);
    }
}
