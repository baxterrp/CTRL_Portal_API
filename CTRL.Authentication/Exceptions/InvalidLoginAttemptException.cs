using System;

namespace CTRL.Authentication.Exceptions
{
    public class InvalidLoginAttemptException : Exception
    {
        public InvalidLoginAttemptException(string message): base(message)
        {
        }
    }
}
