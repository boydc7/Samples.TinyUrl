using System.Collections.Concurrent;
using Samples.TinyUrl.Common.Interfaces;

namespace Samples.TinyUrl.Domain.Memory.DataAccess;

internal abstract class BaseInMemoryRepository<T>
    where T : IHasStringId
{
    private readonly ConcurrentDictionary<string, T> _entities = new(StringComparer.Ordinal);

    protected bool TryAdd(string id, T entity)
        => _entities.TryAdd(id, entity);

    protected bool TryDelete(string id)
        => _entities.TryRemove(id, out _);

    protected T? Get(string id)
        => _entities.GetValueOrDefault(id);
}
