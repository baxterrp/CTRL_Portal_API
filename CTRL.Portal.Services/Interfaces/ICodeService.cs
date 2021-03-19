using CTRL.Portal.Data.DTO;
using System.Threading.Tasks;

namespace CTRL.Portal.Services.Interfaces
{
    public interface ICodeService
    {
        Task<PersistedCodeDto> SaveCode(string email);
        Task<bool> ValidateCode(string email, string code);
    }
}
