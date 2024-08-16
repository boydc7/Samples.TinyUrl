namespace Samples.TinyUrl.Common.Exceptions;

public class TinyUrlDuplicateKeyException : TinyUrlException
{
    public TinyUrlDuplicateKeyException(string message, Exception? innerException = null) 
        : base(message, innerException) { }
}
