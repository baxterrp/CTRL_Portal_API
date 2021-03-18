using System.Threading.Tasks;

namespace CTRL.Portal.Services.Interfaces
{
    public interface IViewRenderService
    {
        Task<string> RenderToStringAsync(string viewName, object model);
    }
}
