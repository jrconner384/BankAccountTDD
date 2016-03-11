using System;

namespace EffinghamLibrary.Helpers
{
    public class InvalidAccountTypeException : ApplicationException
    {
        private const string DefaultErrorMessage = "The account type specified by the AccountData.AccountType property is not valid";

        public InvalidAccountTypeException()
            : base(DefaultErrorMessage)
        {
            
        }

        public InvalidAccountTypeException(string message)
            : base(message)
        {
            
        }

        public InvalidAccountTypeException(string message, Exception innerException)
            : base(message, innerException)
        {
            
        }
    }
}
