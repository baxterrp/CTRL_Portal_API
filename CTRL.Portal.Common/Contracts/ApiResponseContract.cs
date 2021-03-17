using System.Net;

namespace CTRL.Portal.Common.Contracts
{
    public class ApiResponseContract
    {
        public HttpStatusCode Status { get; set; }
        public string Message { get; set; }
    }
}
