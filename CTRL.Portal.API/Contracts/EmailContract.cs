namespace CTRL.Portal.API.Contracts
{
    public class EmailContract
    {
        public string Name { get; set; }
        public string Recipient { get; set; }
        public string Message { get; set; }
        public string Header { get; set; }
        public string ViewName { get; set; }
    }
}
