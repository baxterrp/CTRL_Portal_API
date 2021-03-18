using CTRL.Portal.Data.DTO;
using System.Threading.Tasks;

namespace CTRL.Portal.Data.Repositories
{
    public interface ICodeRepository
    {
        Task SaveCode(PersistedCodeDto persistCode);
        Task<PersistedCodeDto> GetCode(string code, string email);
    }
}
