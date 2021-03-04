using CTRL.Portal.API.Contracts;
using System.Threading.Tasks;

namespace CTRL.Portal.API.Services
{
    public interface IEmailProvider
    {
        Task SendEmail<TEmailContract>(TEmailContract emailModel)
            where TEmailContract : EmailContract;
    }
}
