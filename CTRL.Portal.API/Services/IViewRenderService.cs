using System.Threading.Tasks;

namespace CTRL.Portal.API.Services
{
    public interface IViewRenderService
    {
        Task<string> RenderToStringAsync(string viewName, object model);
    }
}
