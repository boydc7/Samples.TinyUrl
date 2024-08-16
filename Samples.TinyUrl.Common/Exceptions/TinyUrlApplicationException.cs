namespace Samples.TinyUrl.Common.Exceptions;

public class TinyUrlApplicationException : TinyUrlException
{
    public TinyUrlApplicationException(string message, Exception? innerException = null)
        : base(message, innerException) { }
}
