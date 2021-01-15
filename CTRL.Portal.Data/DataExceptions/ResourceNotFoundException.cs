using System;

namespace CTRL.Portal.Data.DataExceptions
{
    public class ResourceNotFoundException : Exception
    {
        public ResourceNotFoundException(string message) : base(message)
        {
        }
    }
}
