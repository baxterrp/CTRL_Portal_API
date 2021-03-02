using CTRL.Portal.Data.DTO;
using System.Threading.Tasks;

namespace CTRL.Portal.Data.Repositories
{
    public interface IAccountCodeRepository
    {
        Task<AccountCode> GetAccountId(string code);
        Task SaveAccountCode(AccountCode accountCode);
    }
}
