namespace Samples.TinyUrl.Domain.Abstractions.Services;

public interface ITinyUrlRetrievalStatsService
{
    Task<long> GetRetrievalCountAsync(string forTinyUrl, string? rootDomain = null);
}
