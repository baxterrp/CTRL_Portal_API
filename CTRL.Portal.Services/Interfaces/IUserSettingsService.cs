using CTRL.Portal.Data.DTO;
using System.Threading.Tasks;

namespace CTRL.Portal.Services.Interfaces
{
    public interface IUserSettingsService
    {
        Task SaveSettings(UserSettingsDto userSettings);
        Task<UserSettingsDto> GetUserSettings(string userName);
    }
}
