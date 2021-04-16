using CTRL.Portal.Data.DTO;
using System.Threading.Tasks;

namespace CTRL.Portal.Data.Repositories
{
    public interface IBusinessEntityCodeRepository
    {
        Task<BusinessEntityCode> GetAccountCode(string code);
        Task SaveAccountCode(BusinessEntityCode accountCode);
        Task UpdateCodeStatus(string code);
    }
}
