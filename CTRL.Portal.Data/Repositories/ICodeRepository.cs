using CTRL.Portal.Data.DTO;
using System.Threading.Tasks;

namespace CTRL.Portal.Data.Repositories
{
    public interface ICodeRepository
    {
        Task SaveCode(PersistedCode persistCode);
        Task<PersistedCode> GetCode(string code);
    }
}
