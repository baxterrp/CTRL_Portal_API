using System.Net;

namespace CTRL.Portal.API.Contracts
{
    public class ApiResponseContract
    {
        public HttpStatusCode Status { get; set; }
        public string Message { get; set; }
    }
}
