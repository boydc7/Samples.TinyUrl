namespace Samples.TinyUrl.Common.Exceptions;

public class TinyUrlNotFoundException : TinyUrlException
{
    public TinyUrlNotFoundException(string message, Exception? innerException = null) 
        : base(message, innerException) { }
}
