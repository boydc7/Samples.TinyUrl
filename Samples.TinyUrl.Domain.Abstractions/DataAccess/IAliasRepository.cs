using Samples.TinyUrl.Domain.Abstractions.Models;

namespace Samples.TinyUrl.Domain.Abstractions.DataAccess;

public interface IAliasRepository
{
    public Task CreateAliasAsync(Alias alias);
    public Task<bool> DeleteAliasAsync(string alias, string? root);
    public Task<string?> GetOriginalAsync(string alias, string? root);
}
