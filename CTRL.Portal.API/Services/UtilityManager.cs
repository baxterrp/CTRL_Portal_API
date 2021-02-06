using System;

namespace CTRL.Portal.API.Services
{
    public class UtilityManager : IUtilityManager
    {
        private static readonly string _characters = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";

        public string GenerateCode(int codeLength)
        {
            var code = string.Empty;
            var random = new Random();

            for(var i = 0; i < codeLength; i++)
            {
                var index = random.Next(_characters.Length);
                code += _characters[index]; 
            }

            return code;
        }
    }
}
