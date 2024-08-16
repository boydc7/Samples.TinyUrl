using System.Runtime.CompilerServices;
using Microsoft.Extensions.DependencyInjection;
using Samples.TinyUrl.Domain.Abstractions.DataAccess;
using Samples.TinyUrl.Domain.Abstractions.Services;
using Samples.TinyUrl.Domain.Memory.DataAccess;
using Samples.TinyUrl.Domain.Memory.Services;

[assembly: InternalsVisibleTo("Samples.TinyUrl.UnitTests")]

namespace Samples.TinyUrl.Domain.Memory;

public static class DomainMemoryExtensions
{
    public static IServiceCollection AddInMemoryDomainDataAccess(this IServiceCollection serviceCollection)
        => serviceCollection.AddSingleton<IAliasRepository, InMemoryAliasRepository>()
                            .AddSingleton<IRootRepository, InMemoryRootRepository>();

    public static IServiceCollection AddInMemoryBaseConversionTinyIdGenerator(this IServiceCollection serviceCollection)
        => serviceCollection.AddSingleton<ITinyIdGenerator, InMemoryBaseConversionTinyIdGenerator>();

    public static IServiceCollection AddInMemorySequentialTinyIdGenerator(this IServiceCollection serviceCollection)
        => serviceCollection.AddSingleton<ITinyIdGenerator, InMemorySequentialTinyIdGenerator>();

    public static IServiceCollection AddInMemorySequenceGenerator(this IServiceCollection serviceCollection)
        => serviceCollection.AddSingleton(InMemorySequenceProvider.Instance);

    public static IServiceCollection AddInMemoryTrackedRetrievalService(this IServiceCollection serviceCollection)
        => serviceCollection.AddSingleton<InMemoryTinyUrlRetrievalService>()
                            .AddSingleton(p => new InMemoryTrackedTinyUrlRetrievalService(p.GetRequiredService<InMemoryTinyUrlRetrievalService>(),
                                                                                          p.GetRequiredService<IRootDomainService>()))
                            .AddSingleton<ITinyUrlRetrievalService>(p => p.GetRequiredService<InMemoryTrackedTinyUrlRetrievalService>())
                            .AddSingleton<ITinyUrlRetrievalStatsService>(p => p.GetRequiredService<InMemoryTrackedTinyUrlRetrievalService>());
}
