using System.Collections.Concurrent;
using Samples.TinyUrl.Domain.Abstractions.Services;

namespace Samples.TinyUrl.Domain.Memory.Services;

public class InMemorySequenceProvider : ISequenceGenerator
{
    private readonly ConcurrentDictionary<string, long> _sequenceMap = new(StringComparer.OrdinalIgnoreCase);

    public static ISequenceGenerator Instance { get; } = Create();

    public static InMemorySequenceProvider Create() => new();

    public ValueTask<long> IncrementAsync(string key, long amount = 1)
    {
        var returnValue = _sequenceMap.AddOrUpdate(key,
                                                   _ => 100_000_123,
                                                   (_, x) => x + amount);

        return new ValueTask<long>(returnValue);
    }
}
