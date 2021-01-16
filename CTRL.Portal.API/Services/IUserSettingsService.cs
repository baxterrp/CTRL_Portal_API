using CTRL.Portal.Data.DTO;
using System.Threading.Tasks;

namespace CTRL.Portal.API.Services
{
    public interface IUserSettingsService
    {
        Task SaveSettings(UserSettings userSettings);
        Task<UserSettings> GetUserSettings(string userName);
    }
}
