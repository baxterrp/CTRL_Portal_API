namespace CTRL.Portal.Data.DTO
{
    public class AccountCode : BaseDto
    {
        public string AccountId { get; set; }
        public string CodeId { get; set; }
        public bool Accepted { get; set; }
    }
}
