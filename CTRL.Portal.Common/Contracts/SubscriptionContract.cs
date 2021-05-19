using System.Collections.Generic;

namespace CTRL.Portal.Common.Contracts
{
    public class SubscriptionContract
    {
        public string BusinessEntityId { get; set; }
        public string Name { get; set; }
        public List<string> ModuleIds { get; set; }
    }
}
