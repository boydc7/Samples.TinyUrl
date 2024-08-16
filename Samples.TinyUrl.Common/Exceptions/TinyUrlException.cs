namespace Samples.TinyUrl.Common.Exceptions;

public abstract class TinyUrlException : Exception
{
    public TinyUrlException(string message, Exception? innerException = null)
        : base(string.IsNullOrEmpty(message)
                   ? innerException?.Message
                   : message,
               innerException) { }
}
