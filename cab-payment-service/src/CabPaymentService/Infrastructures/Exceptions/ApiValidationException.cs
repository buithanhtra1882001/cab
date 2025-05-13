namespace CabPaymentService.Infrastructures.Exceptions
{
    public class ApiValidationException : Exception
    {
        public ApiValidationException(string message)
        : base(message)
        {
        }

        public ApiValidationException(string message, Exception ex)
        : base(message, ex)
        {
        }
    }
}
