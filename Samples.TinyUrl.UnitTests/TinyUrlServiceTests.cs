using System.Collections.Concurrent;
using Microsoft.Extensions.Logging.Abstractions;
using Samples.TinyUrl.Common.Exceptions;
using Samples.TinyUrl.Domain.Abstractions.Enums;
using Samples.TinyUrl.Domain.Core.Services;
using Samples.TinyUrl.Domain.Memory.DataAccess;
using Samples.TinyUrl.Domain.Memory.Services;

namespace Samples.TinyUrl.UnitTests;

public class TinyUrlServiceTests
{
    private readonly InMemorySequenceProvider _inMemorySequenceProvider = InMemorySequenceProvider.Create();
    private readonly InMemoryAliasRepository _inMemoryAliasRepository = new();
    private readonly InMemoryRootRepository _inMemoryRootRepository = new();
    
    private InMemoryBaseConversionTinyIdGenerator _tinyIdGenerator;
    private RootDomainService _rootDomainService;
    private InMemoryTinyUrlRetrievalService _inMemoryTinyUrlRetrievalService;
    private TinyUrlService _tinyUrlService;

    [OneTimeSetUp]
    public void OneTimeSetUp()
    {
        _rootDomainService = new RootDomainService(_inMemoryRootRepository);

        _tinyIdGenerator = new InMemoryBaseConversionTinyIdGenerator(_inMemorySequenceProvider);

        _inMemoryTinyUrlRetrievalService = new InMemoryTinyUrlRetrievalService(_inMemoryAliasRepository);

        _tinyUrlService = new TinyUrlService(_tinyIdGenerator, _inMemoryTinyUrlRetrievalService,
                                             _inMemoryAliasRepository, _rootDomainService,
                                             NullLogger<TinyUrlService>.Instance);
    }
    
    [Test]
    public async Task CanCreateTinyUrlAlone()
    {
        var tinyUrl = await _tinyUrlService.CreateTinyUrlAsync(nameof(CanCreateTinyUrlAlone));

        Assert.That(tinyUrl is { Length: TinyIds.Length });
    }
    
    [Test]
    public async Task CanRetrieveLongUrlFromTinyUrl()
    {
        var tinyUrl = await _tinyUrlService.CreateTinyUrlAsync(nameof(CanRetrieveLongUrlFromTinyUrl));

        Assert.That(tinyUrl is { Length: TinyIds.Length });

        var longUrl = await _tinyUrlService.GetLongUrlAsync(tinyUrl);

        Assert.That(longUrl, Is.EqualTo(nameof(CanRetrieveLongUrlFromTinyUrl)));
    }
    
    [Test]
    public async Task CanCreateAndGetUniqueTinyUrlsInParallel()
    {
        var failedOn = (Task: 0, TinyUrl: string.Empty);
        var generated = new ConcurrentDictionary<string, string>(StringComparer.Ordinal);
        var queuedTinyUrls = new ConcurrentQueue<string>();

        var tasks = Enumerable.Range(1, 5)
                              .Select(i => Task.Run(async () =>
                                                    {
                                                        for (var n = 0; n < 5000; n++)
                                                        {
                                                            if (failedOn.Task > 0)
                                                            {
                                                                break;
                                                            }

                                                            var longUrl = string.Concat(nameof(CanCreateAndGetUniqueTinyUrlsInParallel), "|", n, "|", i);
                                                            
                                                            var id = await _tinyUrlService.CreateTinyUrlAsync(longUrl);

                                                            if (!generated.TryAdd(id, longUrl))
                                                            {
                                                                failedOn = (i, id);

                                                                break;
                                                            }

                                                            queuedTinyUrls.Enqueue(id);
                                                        }
                                                    }))
                              .ToArray();

        await Task.WhenAll(tasks);

        Assert.Multiple(() =>
                        {
                            Assert.That(failedOn.Task, Is.LessThanOrEqualTo(0), $"Failed storing urls in parallel - [{failedOn.TinyUrl}]");
                            Assert.That(generated, Has.Count.GreaterThanOrEqualTo(25000));
                        });
        
        // Now go and get those things in parallel
        var getTasks = Enumerable.Range(1, 5)
                              .Select(i => Task.Run(async () =>
                                                    {
                                                        while (queuedTinyUrls.TryDequeue(out var tinyUrl))
                                                        {
                                                            if (failedOn.Task > 0)
                                                            {
                                                                break;
                                                            }
                                                            
                                                            if (!generated.TryGetValue(tinyUrl, out var generatedLongUrl))
                                                            {
                                                                failedOn = (i, tinyUrl);

                                                                break;
                                                            }
                                                            
                                                            var retrievedLongUrl = await _tinyUrlService.GetLongUrlAsync(tinyUrl);

                                                            if (string.IsNullOrEmpty(retrievedLongUrl) ||
                                                                !retrievedLongUrl.Equals(generatedLongUrl, StringComparison.Ordinal))
                                                            {
                                                                failedOn = (i, tinyUrl);

                                                                break;
                                                            }
                                                        }
                                                    }))
                              .ToArray();

        await Task.WhenAll(getTasks);

        Assert.Multiple(() =>
                        {
                            Assert.That(failedOn.Task, Is.LessThanOrEqualTo(0), $"Failed retrieving urls in parallel - [{failedOn.TinyUrl}]");
                            Assert.That(queuedTinyUrls.IsEmpty, Is.True);
                        });
    }
    
    [Test]
    public void CreateTinyUrlWithMissingRootFails()
    {
        Assert.ThrowsAsync<TinyUrlNotFoundException>(async () => await _tinyUrlService.CreateTinyUrlAsync(nameof(CreateTinyUrlWithMissingRootFails),
                                                                                                          "z_missingInDomain_z"));
    }
    
    [Test]
    public async Task CanCreateTinyUrlWithCustomAlias()
    {
        const string customAlias = "abc-123";
        
        var tinyUrl = await _tinyUrlService.CreateTinyUrlAsync(nameof(CanCreateTinyUrlWithCustomAlias),
                                                               atTinyUrl: customAlias);

        Assert.That(tinyUrl != null && tinyUrl.Equals(customAlias, StringComparison.Ordinal));
    }

    [Test]
    public async Task TryingToCreateTinyUrlWithCustomAliasThatExistsFails()
    {
        const string customAlias = "abc-123-fail";
        
        var tinyUrl = await _tinyUrlService.CreateTinyUrlAsync(nameof(TryingToCreateTinyUrlWithCustomAliasThatExistsFails),
                                                               atTinyUrl: customAlias);

        Assert.That(tinyUrl != null && tinyUrl.Equals(customAlias, StringComparison.Ordinal));

        Assert.ThrowsAsync<TinyUrlDuplicateKeyException>(async () => await _tinyUrlService.CreateTinyUrlAsync(nameof(TryingToCreateTinyUrlWithCustomAliasThatExistsFails),
                                                                                                              atTinyUrl: customAlias));
    }

    [Test]
    public async Task CanDeleteCreatedTinyUrl()
    {
        var tinyUrl = await _tinyUrlService.CreateTinyUrlAsync(nameof(CanDeleteCreatedTinyUrl));

        Assert.That(tinyUrl is { Length: TinyIds.Length });

        var deleted = await _tinyUrlService.DeleteAsync(tinyUrl);

        Assert.That(deleted, Is.True);
        
        // Do it again, should not fail but return false
        var deletedAgain = await _tinyUrlService.DeleteAsync(tinyUrl);
        
        Assert.That(deletedAgain, Is.False);
        
        // Retrieval should return null
        var noTinyUrl = await _tinyUrlService.GetLongUrlAsync(tinyUrl);

        Assert.That(noTinyUrl, Is.Null);
    }
    
    [Test]
    public async Task CanGetStatsOnTrackedTinyUrlRetrieval()
    {
        var trackedRetrievalService = new InMemoryTrackedTinyUrlRetrievalService(_inMemoryTinyUrlRetrievalService, _rootDomainService);
        
        var trackedTinyUrlService = new TinyUrlService(_tinyIdGenerator, trackedRetrievalService,
                                                       _inMemoryAliasRepository, _rootDomainService,
                                                       NullLogger<TinyUrlService>.Instance);

        // Create
        var tinyUrlRetrieved = await trackedTinyUrlService.CreateTinyUrlAsync(nameof(CanGetStatsOnTrackedTinyUrlRetrieval));
        var tinyUrlNotRetrieved = await trackedTinyUrlService.CreateTinyUrlAsync(string.Concat(nameof(CanGetStatsOnTrackedTinyUrlRetrieval), "-nope"));

        for (var i = 1; i <= 100; i++)
        {
            var longUrl = await trackedTinyUrlService.GetLongUrlAsync(tinyUrlRetrieved);

            Assert.That(longUrl, Is.EqualTo(nameof(CanGetStatsOnTrackedTinyUrlRetrieval)));

            var hits = await trackedRetrievalService.GetRetrievalCountAsync(tinyUrlRetrieved);
            
            Assert.That(hits, Is.EqualTo(i));
        }
        
        var noHits = await trackedRetrievalService.GetRetrievalCountAsync(tinyUrlNotRetrieved);
            
        Assert.That(noHits, Is.EqualTo(0));
    }
}
