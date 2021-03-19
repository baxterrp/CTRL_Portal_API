using CTRL.Portal.Data.DTO;
using CTRL.Portal.Data.Repositories;
using CTRL.Portal.Services.Interfaces;
using System;
using System.Threading.Tasks;

namespace CTRL.Portal.Services.Implementation
{
    public class UserSettingsService : IUserSettingsService
    {
        private readonly IUserSettingsRepository _userSettingsRepository;

        public UserSettingsService(IUserSettingsRepository userSettingsRepository)
        {
            _userSettingsRepository = userSettingsRepository ?? throw new ArgumentNullException(nameof(userSettingsRepository));
        }

        public async Task<UserSettingsDto> GetUserSettings(string userName)
        {
            if (string.IsNullOrWhiteSpace(userName))
            {
                throw new ArgumentNullException(nameof(userName));
            }

            return await _userSettingsRepository.GetUserSettings(userName);
        }

        public async Task SaveSettings(UserSettingsDto userSettings)
        {
            if (userSettings is null)
            {
                throw new ArgumentNullException(nameof(userSettings));
            }

            if(string.IsNullOrWhiteSpace(userSettings.UserName))
            {
                throw new ArgumentException("Username cannot be null or empty", nameof(userSettings.UserName));
            }

            await _userSettingsRepository.SaveSettings(userSettings);
        }
    }
}
