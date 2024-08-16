using Samples.TinyUrl.Common.Exceptions;
using Samples.TinyUrl.Domain.Abstractions.DataAccess;
using Samples.TinyUrl.Domain.Abstractions.Models;

namespace Samples.TinyUrl.Domain.Memory.DataAccess;

internal class InMemoryAliasRepository : BaseInMemoryRepository<Alias>, IAliasRepository
{
    public Task CreateAliasAsync(Alias alias)
    {
        if (TryAdd(GetAliasEntityId(alias), alias))
        {
            return Task.CompletedTask;
        }

        throw new TinyUrlDuplicateKeyException($"Alias with ID [{alias.Id}] already exists");
    }

    public Task<bool> DeleteAliasAsync(string alias, string? root)
    {
        var deleted = TryDelete(GetAliasEntityId(alias, root));

        return Task.FromResult(deleted);
    }

    public Task<string?> GetOriginalAsync(string alias, string? root)
    {
        var aliasEntity = Get(GetAliasEntityId(alias, root));

        return Task.FromResult(aliasEntity?.OriginalValue);
    }

    private string GetAliasEntityId(Alias alias)
        => GetAliasEntityId(alias.Id, alias.RootId);

    private string GetAliasEntityId(string alias, string? root)
        => string.Concat(root, "|", alias);
}
