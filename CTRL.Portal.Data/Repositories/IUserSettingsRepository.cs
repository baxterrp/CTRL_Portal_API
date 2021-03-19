using CTRL.Portal.Data.DTO;
using System.Threading.Tasks;

namespace CTRL.Portal.Data.Repositories
{
    public interface IUserSettingsRepository
    {
        Task SaveSettings(UserSettingsDto userSettings);
        Task<UserSettingsDto> GetUserSettings(string userName);
    }
}
