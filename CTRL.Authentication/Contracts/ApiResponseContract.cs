using System.Net;

namespace CTRL.Authentication.Contracts
{
    public class ApiResponseContract
    {
        public HttpStatusCode Status { get; set; }
        public string Message { get; set; }
    }
}
