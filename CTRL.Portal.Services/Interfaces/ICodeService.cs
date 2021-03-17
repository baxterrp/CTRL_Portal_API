using CTRL.Portal.Data.DTO;
using System.Threading.Tasks;

namespace CTRL.Portal.Services.Interfaces
{
    public interface ICodeService
    {
        Task<PersistedCode> SaveCode(string email);
        Task<bool> ValidateCode(string email, string code);
    }
}
