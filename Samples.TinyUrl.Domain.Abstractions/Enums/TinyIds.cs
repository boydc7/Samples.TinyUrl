namespace Samples.TinyUrl.Domain.Abstractions.Enums;

public static class TinyIds
{
    // Length of 7 with base62 gives us 3.52 trillion and change
    // Length of 8 with base62 gives us 218.34 trillion and change
    public const int Length = 7;
}
