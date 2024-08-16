namespace Samples.TinyUrl.Domain.Abstractions.Services;

public interface ITinyUrlRetrievalService
{
    Task<string?> GetUrlAsync(string forTinyUrl, string rootDomain);
}
