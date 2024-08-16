using Samples.TinyUrl.Common.Exceptions;
using Samples.TinyUrl.Domain.Abstractions.DataAccess;
using Samples.TinyUrl.Domain.Abstractions.Models;

namespace Samples.TinyUrl.Domain.Memory.DataAccess;

internal class InMemoryRootRepository : BaseInMemoryRepository<Root>, IRootRepository
{
    public Task CreateRootAsync(Root root)
    {
        if (TryAdd(root.Id, root))
        {
            return Task.CompletedTask;
        }

        throw new TinyUrlDuplicateKeyException($"Root with ID [{root.Id}] already exists");
    }

    public Task<Root?> GetRootAsync(string id)
    {
        var root = Get(id);

        return Task.FromResult(root);
    }
}
