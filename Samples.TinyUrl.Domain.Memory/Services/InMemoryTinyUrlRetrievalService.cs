using Samples.TinyUrl.Domain.Abstractions.DataAccess;
using Samples.TinyUrl.Domain.Abstractions.Services;

namespace Samples.TinyUrl.Domain.Memory.Services;

internal class InMemoryTinyUrlRetrievalService : ITinyUrlRetrievalService
{
    private readonly IAliasRepository _aliasRepository;

    public InMemoryTinyUrlRetrievalService(IAliasRepository aliasRepository)
    {
        _aliasRepository = aliasRepository;
    }

    public async Task<string?> GetUrlAsync(string forTinyUrl, string rootDomain)
    {
        var longUrl = await _aliasRepository.GetOriginalAsync(forTinyUrl, rootDomain);

        return longUrl;
    }
}
