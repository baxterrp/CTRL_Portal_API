using CTRL.Portal.Data.DTO;
using CTRL.Portal.Data.Repositories;
using System;
using System.Threading.Tasks;

namespace CTRL.Portal.API.Services
{
    public class UserSettingsService : IUserSettingsService
    {
        private readonly IUserSettingsRepository _userSettingsRepository;

        public UserSettingsService(IUserSettingsRepository userSettingsRepository)
        {
            _userSettingsRepository = userSettingsRepository ?? throw new ArgumentNullException(nameof(userSettingsRepository));
        }

        public async Task<UserSettings> GetUserSettings(string userName)
        {
            if (string.IsNullOrWhiteSpace(userName)) throw new ArgumentNullException(nameof(userName));
            return await _userSettingsRepository.GetUserSettings(userName);
        }

        public async Task SaveSettings(UserSettings userSettings)
        {
            if (userSettings is null
                || string.IsNullOrWhiteSpace(userSettings?.UserName))
            throw new ArgumentException(nameof(userSettings));

            await _userSettingsRepository.SaveSettings(userSettings);
        }
    }
}
