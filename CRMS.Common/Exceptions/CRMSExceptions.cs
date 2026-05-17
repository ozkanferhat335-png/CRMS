using System;

namespace CRMS.Common.Exceptions
{
    public class CRMSException : Exception
    {
        public CRMSException(string message) : base(message) { }
        public CRMSException(string message, Exception innerException) : base(message, innerException) { }
    }

    public class AuthenticationException : CRMSException
    {
        public AuthenticationException(string message) : base(message) { }
        public AuthenticationException(string message, Exception innerException) : base(message, innerException) { }
    }

    public class AuthorizationException : CRMSException
    {
        public AuthorizationException(string message) : base(message) { }
        public AuthorizationException(string message, Exception innerException) : base(message, innerException) { }
    }

    public class ValidationException : CRMSException
    {
        public ValidationException(string message) : base(message) { }
        public ValidationException(string message, Exception innerException) : base(message, innerException) { }
    }

    public class DataAccessException : CRMSException
    {
        public DataAccessException(string message) : base(message) { }
        public DataAccessException(string message, Exception innerException) : base(message, innerException) { }
    }
}