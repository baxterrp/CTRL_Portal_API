using CTRL.Portal.Data.DTO;
using CTRL.Portal.Data.Repositories;
using CTRL.Portal.Services.Configuration;
using CTRL.Portal.Services.Interfaces;
using System;
using System.Threading.Tasks;

namespace CTRL.Portal.Services.Implementation
{
    public class CodeService : ICodeService
    {
        private readonly CodeConfiguration _codeConfiguration;
        private readonly ICodeRepository _codeRepository;
        private readonly IUtilityManager _utilityManager;

        public CodeService(CodeConfiguration codeConfiguration, ICodeRepository codeRepository, IUtilityManager utilityManager)
        {
            _codeConfiguration = codeConfiguration ?? throw new ArgumentNullException(nameof(codeConfiguration));
            _codeRepository = codeRepository ?? throw new ArgumentNullException(nameof(codeRepository));
            _utilityManager = utilityManager ?? throw new ArgumentNullException(nameof(utilityManager));
        }

        public async Task<PersistedCode> SaveCode(string email)
        {
            var code = new PersistedCode
            {
                Code = _utilityManager.GenerateCode(),
                Expiration = DateTime.Now.Add(TimeSpan.Parse(_codeConfiguration.Expires)),
                Id = Guid.NewGuid().ToString(),
                Email = email
            };

            await _codeRepository.SaveCode(code);

            return code;
        }

        public async Task<bool> ValidateCode(string email, string code)
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                throw new ArgumentNullException(nameof(email));
            }

            if (string.IsNullOrWhiteSpace(code))
            {
                throw new ArgumentNullException(nameof(code));
            }

            var actualCode = await _codeRepository.GetCode(code, email);

            if (actualCode is null)
            {
                return false;
            }

            return
                actualCode.Email == email
                && actualCode.Expiration >= DateTime.Now
                && actualCode.Code == code;
        }
    }
}
