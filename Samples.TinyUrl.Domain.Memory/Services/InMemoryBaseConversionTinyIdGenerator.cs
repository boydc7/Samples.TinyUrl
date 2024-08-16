using Samples.TinyUrl.Common.Extensions;
using Samples.TinyUrl.Domain.Abstractions.Enums;
using Samples.TinyUrl.Domain.Abstractions.Services;

namespace Samples.TinyUrl.Domain.Memory.Services;

// Generates a string of a set length for an 64-bit integer returned from a generator. Uses a simple base conversion
// (i.e. from base10 to baseX) where X is the length of the set token array (62 using numbers and letters only).
internal class InMemoryBaseConversionTinyIdGenerator : ITinyIdGenerator
{
    private readonly char[] _tokens = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789".ToCharArray();
    private readonly int _tokenLength;
    private readonly ISequenceGenerator _sequenceGenerator;

    public InMemoryBaseConversionTinyIdGenerator(ISequenceGenerator sequenceGenerator)
    {
        _sequenceGenerator = sequenceGenerator;

        _tokenLength = _tokens.Length;
    }

    public async ValueTask<string> GetIdAsync(string? group = null)
    {
        var sequence = await _sequenceGenerator.IncrementAsync(group.Coalesce(Sequences.GlobalSequenceKey));

        var idBuffer = ToBaseX(sequence);

        var id = new string(idBuffer);

        return id;
    }

    private char[] ToBaseX(long sequence)
    {
        var buffer = new char[TinyIds.Length];
        var index = TinyIds.Length - 1;

        do
        {
            buffer[index] = _tokens[(sequence % _tokenLength)];
            sequence /= _tokenLength;
            index--;
        } while (sequence > 0 && index >= 0);

        // Pad with 0's to length
        while (index >= 0)
        {
            buffer[index] = '0';
            index--;
        }

        return buffer;
    }
}
