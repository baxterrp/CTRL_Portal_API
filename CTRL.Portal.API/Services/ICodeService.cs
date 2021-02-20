using CTRL.Portal.Data.DTO;
using System.Threading.Tasks;

namespace CTRL.Portal.API.Services
{
    public interface ICodeService
    {
        Task<PersistedCode> SaveCode(string email);
        Task<bool> ValidateCode(string email, string code);
    }
}
