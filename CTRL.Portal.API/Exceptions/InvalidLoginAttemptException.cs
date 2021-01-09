using System;

namespace CTRL.Portal.API.Exceptions
{
    public class InvalidLoginAttemptException : Exception
    {
        public InvalidLoginAttemptException(string message): base(message)
        {
        }
    }
}
