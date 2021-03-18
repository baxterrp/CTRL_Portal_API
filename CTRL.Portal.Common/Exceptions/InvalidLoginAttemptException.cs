using System;

namespace CTRL.Portal.Common.Exceptions
{
    public class InvalidLoginAttemptException : Exception
    {
        public InvalidLoginAttemptException(string message): base(message)
        {
        }
    }
}
