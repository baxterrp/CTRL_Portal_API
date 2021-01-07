using System.Net;

namespace CTRL.Inventory.API.Contracts
{
    public class ApiResponseContract
    {
        public HttpStatusCode Status { get; set; }
        public string Message { get; set; }
    }
}
