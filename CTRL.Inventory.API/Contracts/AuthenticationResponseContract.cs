namespace CTRL.Inventory.API.Contracts
{
    public class AuthenticationResponseContract : ApiResponseContract
    {
        public string Token { get; set; }
        public string UserName { get; set; }
    }
}
