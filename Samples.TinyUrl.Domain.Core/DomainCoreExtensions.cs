using System.Runtime.CompilerServices;
using Microsoft.Extensions.DependencyInjection;
using Samples.TinyUrl.Domain.Abstractions.Services;
using Samples.TinyUrl.Domain.Core.Services;

[assembly: InternalsVisibleTo("Samples.TinyUrl.UnitTests")]

namespace Samples.TinyUrl.Domain.Core;

public static class DomainCoreExtensions
{
    public static IServiceCollection AddDomainCoreTinyUrlService(this IServiceCollection serviceCollection)
        => serviceCollection.AddSingleton<ITinyUrlService, TinyUrlService>()
                            .AddSingleton<IRootDomainService, RootDomainService>();
}
