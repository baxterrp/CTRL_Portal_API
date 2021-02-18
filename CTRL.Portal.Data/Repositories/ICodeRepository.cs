using CTRL.Portal.Data.DTO;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CTRL.Portal.Data.Repositories
{
    public interface ICodeRepository
    {
        Task SaveCode(PersistCode persistCode);
        Task SavePersistCode(string email, PersistCode code);
        //Task<PersistCode> GetPersistCode(string email, PersistCode code);
    }
}
