namespace CTRL.Portal.Common.Contracts
{
    public abstract class EmailContract
    {
        public string Name { get; set; }
        public string Recipient { get; set; }
        public string Header { get; set; }
        public string ViewName { get; set; }
    }
}
