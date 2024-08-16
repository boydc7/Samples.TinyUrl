using Samples.TinyUrl.Domain.Abstractions.DataAccess;
using Samples.TinyUrl.Domain.Abstractions.Models;
using Samples.TinyUrl.Domain.Abstractions.Services;

namespace Samples.TinyUrl.Domain.Core.Services;

internal class RootDomainService : IRootDomainService
{
    private readonly IRootRepository _rootRepository;

    public RootDomainService(IRootRepository rootRepository)
    {
        _rootRepository = rootRepository;
    }

    public async Task<string?> GetRootDomainAsync(string? rootDomain)
    {
        if (rootDomain == null)
        {
            return Root.Default;
        }

        // Ensure the root exists
        var root = await _rootRepository.GetRootAsync(rootDomain);

        return root?.Id;
    }

    public async Task CreateRootDomainAsync(string rootDomain)
    {
        if (string.IsNullOrWhiteSpace(rootDomain))
        {
            throw new ArgumentNullException(nameof(rootDomain));
        }

        var root = new Root
                   {
                       Id = rootDomain
                   };

        await _rootRepository.CreateRootAsync(root);
    }
}
