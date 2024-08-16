using System.Collections.Concurrent;
using Samples.TinyUrl.Domain.Abstractions.Services;

namespace Samples.TinyUrl.Domain.Memory.Services;

internal class InMemoryTrackedTinyUrlRetrievalService : ITinyUrlRetrievalService, ITinyUrlRetrievalStatsService
{
    private readonly ITinyUrlRetrievalService _innerRetrievalService;
    private readonly IRootDomainService _rootDomainService;
    private readonly ConcurrentDictionary<string, long> _getStats = new(StringComparer.Ordinal);

    public InMemoryTrackedTinyUrlRetrievalService(ITinyUrlRetrievalService innerRetrievalService,
                                                  IRootDomainService rootDomainService)
    {
        _innerRetrievalService = innerRetrievalService;
        _rootDomainService = rootDomainService;
    }

    public async Task<string?> GetUrlAsync(string forTinyUrl, string rootDomain)
    {
        var longUrl = await _innerRetrievalService.GetUrlAsync(forTinyUrl, rootDomain);

        if (longUrl != null)
        {
            _getStats.AddOrUpdate(GetStatsKey(forTinyUrl, rootDomain),
                                  addValue: 1,
                                  updateValueFactory: (_, x) => (x + 1));
        }

        return longUrl;
    }

    public async Task<long> GetRetrievalCountAsync(string forTinyUrl, string? inDomain = null)
    {
        var rootDomain = await _rootDomainService.GetRootDomainAsync(inDomain);

        var stat = _getStats.GetValueOrDefault(GetStatsKey(forTinyUrl, rootDomain));

        return stat;
    }

    private string GetStatsKey(string forTinyUrl, string? rootDomain = null)
        => string.Concat(rootDomain ?? "__NULLTUDOMAIN__", "|", forTinyUrl);
}
