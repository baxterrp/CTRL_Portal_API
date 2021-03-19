using CTRL.Portal.Common.Contracts;
using System.Threading.Tasks;

namespace CTRL.Portal.Services.Interfaces
{
    public interface IEmailProvider
    {
        Task SendEmail<TEmailContract>(TEmailContract emailModel)
            where TEmailContract : EmailContract;
    }
}
