namespace CabMediaService.Infrastructures.Exceptions
{
    public class CommandHandlerException : Exception
    {
        public CommandHandlerException(string message)
        : base(message)
        {
        }

        public CommandHandlerException(string message, Exception ex)
        : base(message, ex)
        {
        }
    }
}
