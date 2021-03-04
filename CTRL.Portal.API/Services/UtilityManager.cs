using CTRL.Portal.API.Configuration;
using System;

namespace CTRL.Portal.API.Services
{
    public class UtilityManager : IUtilityManager
    {
        private readonly CodeConfiguration _codeConfiguration;

        public UtilityManager(CodeConfiguration codeConfiguration)
        {
            _codeConfiguration = codeConfiguration ?? throw new ArgumentNullException(nameof(codeConfiguration));
        }

        public string GenerateCode()
        {
            var code = string.Empty;
            var random = new Random();

            for(var i = 0; i < _codeConfiguration.Length; i++)
            {
                var index = random.Next(_codeConfiguration.Pattern.Length);
                code += _codeConfiguration.Pattern[index]; 
            }

            return code;
        }
    }
}
