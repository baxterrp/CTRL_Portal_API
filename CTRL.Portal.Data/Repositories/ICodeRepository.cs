using CTRL.Portal.Data.DTO;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CTRL.Portal.Data.Repositories
{
    public interface ICodeRepository
    {
        Task SaveCode(PersistedCode persistCode);
    }
}
