using System;

namespace WCABNetwork.Cab.IdentityService.Infrastructures.Exceptions
{
    public class BusinessException : Exception
    {
        public BusinessException(string message)
        : base(message)
        {
        }

        public BusinessException(string message, Exception ex)
        : base(message, ex)
        {
        }
    }
}
