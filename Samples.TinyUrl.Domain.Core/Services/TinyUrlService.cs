using Microsoft.Extensions.Logging;
using Samples.TinyUrl.Common.Exceptions;
using Samples.TinyUrl.Domain.Abstractions.DataAccess;
using Samples.TinyUrl.Domain.Abstractions.Models;
using Samples.TinyUrl.Domain.Abstractions.Services;

namespace Samples.TinyUrl.Domain.Core.Services;

internal class TinyUrlService : ITinyUrlService
{
    private readonly ITinyIdGenerator _tinyIdGenerator;
    private readonly ITinyUrlRetrievalService _tinyUrlRetrievalService;
    private readonly IAliasRepository _aliasRepository;
    private readonly IRootDomainService _rootDomainService;
    private readonly ILogger<TinyUrlService> _logger;

    public TinyUrlService(ITinyIdGenerator tinyIdGenerator,
                          ITinyUrlRetrievalService tinyUrlRetrievalService,
                          IAliasRepository aliasRepository,
                          IRootDomainService rootDomainService,
                          ILogger<TinyUrlService> logger)
    {
        _tinyIdGenerator = tinyIdGenerator;
        _tinyUrlRetrievalService = tinyUrlRetrievalService;
        _aliasRepository = aliasRepository;
        _rootDomainService = rootDomainService;
        _logger = logger;
    }

    public async Task<string> CreateTinyUrlAsync(string forLongUrl, string? inDomain = null, string? atTinyUrl = null)
    {
        var loops = 0;

        var rootDomain = await GetRootDomainAsync(inDomain);
        
        do
        {
            var id = atTinyUrl ?? await _tinyIdGenerator.GetIdAsync(rootDomain);

            var alias = new Alias
                        {
                            Id = id,
                            OriginalValue = forLongUrl,
                            RootId = rootDomain
                        };

            try
            {
                await _aliasRepository.CreateAliasAsync(alias);

                return id;
            }
            catch(TinyUrlDuplicateKeyException) when(atTinyUrl == null)
            {
                // Re-generate a new ID and continue along
                _logger.LogWarning("TinyUrlCreation resulted in duplicate key of [{Id}]", id);
            }

            loops++;
        } while (loops <= 100);

        throw new TinyUrlApplicationException("TinyUrlCreation could not generate a unique alias map");
    }

    public async Task<string?> GetLongUrlAsync(string forTinyUrl, string? inDomain = null)
    {
        var rootDomain = await GetRootDomainAsync(inDomain);
        
        var result = await _tinyUrlRetrievalService.GetUrlAsync(forTinyUrl, rootDomain);

        return result;
    }

    public async Task<bool> DeleteAsync(string forTinyUrl, string? inDomain = null)
    {
        var rootDomain = await GetRootDomainAsync(inDomain);
        
        var deleted = await _aliasRepository.DeleteAliasAsync(forTinyUrl, rootDomain);

        return deleted;
    }

    private async Task<string> GetRootDomainAsync(string? inDomain)
    {
        var rootDomain = await _rootDomainService.GetRootDomainAsync(inDomain);

        if (rootDomain == null)
        {
            throw new TinyUrlNotFoundException($"RootDomain of [{inDomain}] does not exist");
        }

        return rootDomain;
    }
}
