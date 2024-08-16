namespace Samples.TinyUrl.Domain.Abstractions.Services;

public interface ITinyUrlService
{
    Task<string?> GetLongUrlAsync(string forTinyUrl, string? rootDomain = null);
    Task<string> CreateTinyUrlAsync(string forLongUrl, string? rootDomain = null, string? atTinyUrl = null);
    Task<bool> DeleteAsync(string forTinyUrl, string? rootDomain = null);
}
