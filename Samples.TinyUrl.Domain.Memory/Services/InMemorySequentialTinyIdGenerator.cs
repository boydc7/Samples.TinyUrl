using System.Globalization;
using Samples.TinyUrl.Domain.Abstractions.Services;

namespace Samples.TinyUrl.Domain.Memory.Services;

// Simple generator that just increments an internal 64bit int and returns the string represention
// Should be used for local debugging, testing, simple cases only
// Does not differentiate values for the group passed
internal class InMemorySequentialTinyIdGenerator : ITinyIdGenerator
{
    private long _currentId;

    public ValueTask<string> GetIdAsync(string? group = null)
    {
        var id = Interlocked.Increment(ref _currentId);

        return ValueTask.FromResult(id.ToString(CultureInfo.InvariantCulture));
    }
}
