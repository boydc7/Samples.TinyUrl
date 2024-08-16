namespace Samples.TinyUrl.Common.Extensions;

public static class StringExtensions
{
    public static string? ToNullIfEmpty(this string? source)
        => string.IsNullOrEmpty(source)
               ? null
               : source;

    public static string Coalesce(this string? source, string other)
        => string.IsNullOrEmpty(source)
               ? other
               : source;
}
